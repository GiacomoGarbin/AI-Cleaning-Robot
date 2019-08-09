using System.Collections;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public GameObject RobotPrefab;

    private Robot[] robots;

    public Robot RobotYellow
    {
        get { return robots[GetIndex("RobotYellow")]; }
        private set { robots[GetIndex("RobotYellow")] = value; }
    }

    public Robot RobotMagenta
    {
        get { return robots[GetIndex("RobotMagenta")]; }
        private set { robots[GetIndex("RobotMagenta")] = value; }
    }

    public Robot RobotCyan
    {
        get { return robots[GetIndex("RobotCyan")]; }
        private set { robots[GetIndex("RobotCyan")] = value; }
    }

    int GetIndex(string name)
    {
        switch (name)
        {
            case "RobotYellow":
                return 0;
            case "RobotMagenta":
                return 1;
            case "RobotCyan":
                return 2;
        }
        return -1;
    }

    public TrashCompactor compactor;

    public PowerStation[] stations;

    public Material[] materials;

    void Start()
    {
        robots = new Robot[]
        {
            SpawnRobot("RobotYellow"),
            SpawnRobot("RobotMagenta"), // null,
            SpawnRobot("RobotCyan") // null
        };
    }

    Robot SpawnRobot(string RobotName, int DisposedTrash = 0)
    {
        Robot robot = (Instantiate(RobotPrefab) as GameObject).GetComponent<Robot>();

        /*
        if (RobotName == "RobotCyan")
        {
            robot.WasteRate = 10;
        }
        */

        robot.name = RobotName;
        robot.DisposedTrash = DisposedTrash;
        robot.gameObject.layer = LayerMask.NameToLayer(RobotName);

        int i = GetIndex(RobotName);

        Transform OldParent = robot.transform.parent;
        robot.transform.parent = stations[i].transform;
        robot.transform.localPosition = new Vector3(0, 0, 1);
        robot.transform.localRotation = Quaternion.identity;
        robot.transform.parent = OldParent;

        robot.GetComponent<MeshRenderer>().material = materials[i];
        robot.GetComponent<Robot>().compactor = compactor;
        robot.GetComponent<Robot>().station = stations[i];
        robot.GetComponent<Robot>().station.robot = robot;

        return robot;
    }

    private Coroutine coroutine = null;

    void Update()
    {
        foreach (Robot robot in robots)
        {
            if (CheckPerturbation(robot))
            {
                if (coroutine == null)
                {
                    coroutine = StartCoroutine(RestoreRobot(robot));
                }
            }
        }
    }

    bool CheckPerturbation(Robot robot)
    {
        if (robot == null)
        {
            return false;
        }

        Vector3 rotation = robot.transform.localEulerAngles;
        return CheckAxis(rotation.x) || CheckAxis(rotation.z);
    }

    bool CheckAxis(float axis)
    {
        float epsilon = 10;
        axis = Mathf.Abs(axis % 360);
        // axis is now between 0 (inclusive) and 360
        // check if x is far from 0 or 360 more than epsilon
        return !((axis < 0 + epsilon) || (axis > 360 - epsilon));
    }

    private IEnumerator RestoreRobot(Robot robot)
    {
        // better strategy : check how many seconds the robot has been turned upside down in the last 3 seconds

        yield return new WaitForSeconds(3);

        if (CheckPerturbation(robot))
        {
            // if after 3 seconds the robot is still turned upside down ...
            if (robot.PickedUpObject != null)
            {
                // heavy trash ...
                robot.PickedUpObject.DestroyObject();
            }

            string name = robot.name;
            int DisposedTrash = robot.DisposedTrash;

            // it is not necessary to destroy the robot ...
            robot.DestroyObject();

            robots[GetIndex(name)] = SpawnRobot(name, DisposedTrash);
        }

        coroutine = null;
    }
}
