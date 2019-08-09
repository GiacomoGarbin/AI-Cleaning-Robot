using System.Collections;
using System.Linq;
using UnityEngine;

// public class FormationBehaviour : MonoBehaviour
public class FormationBehaviour
{
    MonoBehaviour MB;

    struct SlotStruct
    {
        public Transform target;
        public Robot robot;

        public SlotStruct(string name)
        {
            target = new GameObject(name).transform;
            robot = null;
        }
    }

    SlotStruct[] SlotList;
    SlotStruct LeaderSlot { get { return SlotList[0]; } set { SlotList[0] = value; } }

    public bool ExpiredTimeout = false;
    public bool CompletedFormation { get { return Enumerable.All(SlotList, slot => slot.robot != null); } }
    public bool FellowLowBattery { get { return Enumerable.Any(SlotList, slot => slot.robot.LowBattery); } }

    float TwoThirdsPI = 2 * Mathf.PI / 3;
    float radius = 1.5f;

    Trash trash;

    public FormationBehaviour(Trash trash, Robot LeaderRobot)
    {
        MB = UnityEngine.Object.FindObjectsOfType<RobotController>()[0].GetComponent<MonoBehaviour>();

        this.trash = trash;

        SlotList = new SlotStruct[3];

        for (int i = 0; i < SlotList.Length; i++)
        {
            SlotList[i] = new SlotStruct("FormationSlot" + i);
            SlotList[i].target.position = SlotPosition(i);
        }

        LiftTrash();

        AddRobot(LeaderRobot);

        float timeout = 10;
        MB.StartCoroutine(StartTimer(timeout));
    }

    Vector3 SlotPosition(int i)
    {
        float angle = Mathf.PI / 2;

        switch (i)
        {
            case 0:
                // angle += 0;
                break;
            case 1:
                angle += TwoThirdsPI;
                break;
            case 2:
                angle -= TwoThirdsPI;
                break;
        }

        return trash.transform.position + radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }

    IEnumerator StartTimer(float seconds)
    {
        // TextMesh label = new TextMesh();
        // label.text = "Hello World!";

        // label.transform.parent = transform;
        // label.transform.localPosition = Vector3.zero;
        // label.transform.localRotation = Quaternion.identity;

        while (seconds > 0 && !CompletedFormation)
        {
            Debug.Log(LeaderSlot.robot.name + ": " + seconds);
            yield return new WaitForSeconds(1);
            seconds--;
        }

        if (seconds == 0)
        {
            ExpiredTimeout = true;
        }
    }

    bool LeaderStillWaitingForFellows { get { return LeaderSlot.robot.behaviour.CurrentState.name == "WaitForFellows"; } }

    float LeaderOldMaxSpeed;

    public void AddRobot(Robot robot)
    {
        if (LeaderSlot.robot == null)
        {
            robot.transform.position = LeaderSlot.target.position;
            robot.transform.rotation = trash.transform.rotation;

            LeaderOldMaxSpeed = robot.steering.maxLinearSpeed;
            robot.steering.maxLinearSpeed = 3;

            SlotList[0].robot = robot;
        }
        else if (SlotList[1].robot == null && LeaderStillWaitingForFellows)
        {
            robot.transform.position = SlotList[1].target.position;
            robot.transform.rotation = trash.transform.rotation;

            SlotArriveBehaviour arrive = robot.gameObject.AddComponent<SlotArriveBehaviour>();
            arrive.TargetPosition = SlotList[1].target;

            robot.GetComponent<AvoidBehaviour>().enabled = false;
            SlotAvoidBehaviour avoid = robot.gameObject.AddComponent<SlotAvoidBehaviour>();
            avoid.SlotIndex = 1;

            SlotList[1].robot = robot;
        }
        else if (SlotList[2].robot == null && LeaderStillWaitingForFellows)
        {
            robot.transform.position = SlotList[2].target.position;
            robot.transform.rotation = trash.transform.rotation;

            SlotArriveBehaviour arrive = robot.gameObject.AddComponent<SlotArriveBehaviour>();
            arrive.TargetPosition = SlotList[2].target;

            robot.GetComponent<AvoidBehaviour>().enabled = false;
            SlotAvoidBehaviour avoid = robot.gameObject.AddComponent<SlotAvoidBehaviour>();
            avoid.SlotIndex = 2;

            SlotList[2].robot = robot;
        }
    }

    public float TrashOffset = 1.25f;

    void LiftTrash()
    {
        Vector3 TrashNewPosition = trash.transform.position;
        TrashNewPosition.y += TrashOffset;

        trash.transform.position = TrashNewPosition;
    }

    public void BreakFormation()
    {
        if (LeaderSlot.robot != null)
        {
            LeaderSlot.robot.steering.maxLinearSpeed = LeaderOldMaxSpeed;

            SlotList[0].robot = null;
        }

        if (SlotList[1].robot != null)
        {
            UnityEngine.Object.Destroy(SlotList[1].robot.GetComponent<SlotArriveBehaviour>());
            UnityEngine.Object.Destroy(SlotList[1].robot.GetComponent<SlotAvoidBehaviour>());
            SlotList[1].robot.GetComponent<AvoidBehaviour>().enabled = true;

            SlotList[1].robot = null;
        }

        if (SlotList[2].robot != null)
        {
            UnityEngine.Object.Destroy(SlotList[2].robot.GetComponent<SlotArriveBehaviour>());
            UnityEngine.Object.Destroy(SlotList[2].robot.GetComponent<SlotAvoidBehaviour>());
            SlotList[2].robot.GetComponent<AvoidBehaviour>().enabled = true;

            SlotList[2].robot = null;
        }

        foreach (SlotStruct slot in SlotList)
        {
            UnityEngine.Object.Destroy(slot.target.gameObject);
        }
    }

    // void Update()
    public void UpdateFormation()
    {
        if (CompletedFormation)
        {
            for (int i = 1; i < SlotList.Length; i++)
            {
                SlotList[i].target.transform.position = UpdateSlotPosition(i);
            }

            UpdateTrashPosition();
        }
    }

    Vector3 UpdateSlotPosition(int i)
    {
        Quaternion rotation = Quaternion.Euler(LeaderSlot.robot.transform.eulerAngles);
        Matrix4x4 RotationMatrix = Matrix4x4.Rotate(rotation);

        return LeaderSlot.robot.transform.position + RotationMatrix.MultiplyPoint3x4(SlotRelativePosition(i));
    }

    Vector3 SlotRelativePosition(int i)
    {
        return SlotPosition(i) - SlotPosition(0);
    }

    void UpdateTrashPosition()
    {
        Vector3 position = GetCenterOfMass();
        position.y += TrashOffset;

        trash.transform.position = position;
        trash.transform.rotation = LeaderSlot.robot.transform.rotation;
    }

    Vector3 GetCenterOfMass()
    {
        Vector3 CenterOfMass = Vector3.zero;

        foreach (SlotStruct slot in SlotList)
        {
            CenterOfMass += slot.robot.transform.position;
        }

        CenterOfMass /= SlotList.Length;

        return CenterOfMass;
    }
}