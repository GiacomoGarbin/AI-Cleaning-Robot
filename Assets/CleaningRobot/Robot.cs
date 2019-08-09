using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Robot : PickableUpObject
{
    public float FieldOfViewRange = 10;
    public float FieldOfViewAngle = 30;

    public TrashCompactor compactor;
    public PowerStation station;

    [HideInInspector] public WanderBehaviour wander;
    [HideInInspector] public SeekBehaviour seek;
    [HideInInspector] public AvoidBehaviour avoid;
    [HideInInspector] public DelegatedSteering steering;

    [HideInInspector] public RobotBehaviour behaviour;
    [HideInInspector] public BlackBoard board;
    [HideInInspector] public FormationBehaviour formation;

    [HideInInspector] public float FullBattery = 1000;
    public float WasteRate = 1;
    public float CurrentBattery;
    public float LowBatteryLevel = 0.15f;
    public bool LowBattery { get { return CurrentBattery < FullBattery * LowBatteryLevel; } }

    public Trash CurrentTrash;
    public Robot FellowRobot;
    public PickableUpObject PickedUpObject;

    [HideInInspector] public int DisposedTrash = 0;

    void Start()
    {
        // PickableUpObject
        PickedUp = false;
        disposed = false;

        wander = GetComponent<WanderBehaviour>();
        seek = GetComponent<SeekBehaviour>();
        avoid = GetComponent<AvoidBehaviour>();
        steering = GetComponent<DelegatedSteering>();

        behaviour = new RobotBehaviour(this);
        board = UnityEngine.Object.FindObjectsOfType<BlackBoard>()[0];

        CurrentBattery = FullBattery;

        CurrentTrash = null;
        PickedUpObject = null;

        behaviour.Start();
    }

    void Update()
    {
        UpdateBattery();

        if (formation != null)
        {
            formation.UpdateFormation();
        }

        behaviour.UpdateBeviour();
    }

    void UpdateBattery()
    {
        float effort = steering.enabled ? (Mathf.Abs(steering.status.linearSpeed) + Mathf.Abs(steering.status.angularSpeed)) * Time.deltaTime : 0;
        CurrentBattery = Mathf.Max(0, CurrentBattery - effort * WasteRate);
    }

    public void AddRobotOnBoard()
    {
        board.AddRobot(this);
    }

    public void StartMovement()
    {
        steering.status = new MovementStatus() { movementDirection = transform.forward };
        GetComponent<Rigidbody>().isKinematic = false;
        steering.enabled = true;
    }

    public void StopMovement()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        steering.enabled = false;
    }

    // set destionation for ... methods

    public void SetDestinationForNothing()
    {
        avoid.LayerMask = ~0;
        seek.destination = null;
    }

    public void SetDestinationForTrash()
    {
        avoid.LayerMask = ~(1 << CurrentTrash.gameObject.layer);
        seek.destination = CurrentTrash.transform;
    }

    public void SetDestinationForTrashCompactor()
    {
        avoid.LayerMask = ~(1 << compactor.gameObject.layer);
        seek.destination = compactor.transform;
    }

    public void SetDestinationForPowerStation()
    {
        avoid.LayerMask = ~(1 << station.gameObject.layer);
        seek.destination = station.transform;
    }

    public void SetDestinationForFellow()
    {
        Debug.Log(name + ": SetDestinationForFellow");

        avoid.LayerMask = ~(1 << FellowRobot.gameObject.layer);
        seek.destination = FellowRobot.transform;
    }

    public void SetDestinationForFellowPowerStation()
    {
        avoid.LayerMask = ~(1 << FellowRobot.station.gameObject.layer);
        seek.destination = FellowRobot.station.transform;
    }

    // pickable-up objects stuff

    public void PickUpLightTrash()
    {
        Debug.Log(name + ": PickUpLightTrash");

        CurrentTrash.transform.parent = transform;
        CurrentTrash.transform.localPosition = new Vector3(0, 1.25f, 0);
        CurrentTrash.transform.localRotation = Quaternion.identity;

        CurrentTrash.PickedUp = true;
        PickedUpObject = CurrentTrash;

        ColliderList.Remove(CurrentTrash.gameObject);
    }

    public void PickUpHeavyTrash()
    {
        Debug.Log(name + ": PickUpHeavyTrash");

        formation = new FormationBehaviour(CurrentTrash, this);

        CurrentTrash.PickedUp = true;
        PickedUpObject = CurrentTrash;

        ColliderList.Remove(CurrentTrash.gameObject);
    }

    public void PickUpFellow()
    {
        FellowRobot.transform.parent = transform;
        FellowRobot.transform.localPosition = new Vector3(0, 1.25f, 0);
        FellowRobot.transform.localRotation = Quaternion.identity;

        FellowRobot.PickedUp = true;
        PickedUpObject = FellowRobot;

        ColliderList.Remove(FellowRobot.gameObject);
    }

    public void PutDownPickedUpObject()
    {
        if (PickedUpObject != null)
        {
            Debug.Log(name + ": PutDownPickedUpObject");

            StartCoroutine(PutDown(PickedUpObject));

            switch (PickedUpObject.tag)
            {
                case "Trash":
                    CurrentTrash = null;
                    break;
                case "Robot":
                    FellowRobot = null;
                    break;
            }

            PickedUpObject = null;
        }
    }

    IEnumerator PutDown(PickableUpObject PickedUpObject)
    {
        PickedUpObject.transform.parent = null;

        Vector3 corner = 0.5f * PickedUpObject.transform.localScale;
        float offset = 1;
        float radius = corner.magnitude + offset;

        int LayerMask = ~((1 << UnityEngine.LayerMask.NameToLayer("Room")) | (1 << PickedUpObject.gameObject.layer));

        while (true)
        {
            yield return new WaitForSeconds(0.001f);

            // check if we hit something in this position
            Collider[] HitColliders = Physics.OverlapSphere(PickedUpObject.transform.position, radius, LayerMask);

            if (HitColliders.Length == 0)
            {
                break;
            }

            string[] tags = new string[] { "Trash", "TrashCompactor", "PowerStation" };

            // check if we hit a power station or the trash compactor or other trash
            if (Enumerable.Any(HitColliders, collider => tags.Contains(collider.gameObject.tag)))
            {
                // move ourselves slightly toward the origin, and check again
                Vector3 direction = new Vector3(0, PickedUpObject.transform.position.y, 0) - PickedUpObject.transform.position;
                PickedUpObject.transform.position += direction.normalized * Time.deltaTime;
            }
        }

        // put down the object
        PickedUpObject.transform.position = new Vector3(PickedUpObject.transform.position.x, 1, PickedUpObject.transform.position.z);
        PickedUpObject.PickedUp = false;
    }

    public void DisposeLightTrash()
    {
        CurrentTrash.transform.parent = compactor.transform;
        CurrentTrash.transform.localPosition = new Vector3(0, 1.25f, 0);
        CurrentTrash.transform.localRotation = Quaternion.identity;

        compactor.AddTrash(CurrentTrash);

        CurrentTrash.disposed = true;
        CurrentTrash = null;
        PickedUpObject = null;

        DisposedTrash++;
    }

    public void DisposeHeavyTrash()
    {
        Debug.Log(name + ": DisposeHeavyTrash");

        CurrentTrash.transform.parent = compactor.transform;
        CurrentTrash.transform.localPosition = new Vector3(0, 1.25f, 0);
        CurrentTrash.transform.localRotation = Quaternion.identity;

        compactor.AddTrash(CurrentTrash);

        CurrentTrash.disposed = true;
        CurrentTrash = null;
        PickedUpObject = null;

        DisposedTrash++;
        // increment score for fellows
    }

    public void ClearCurrentTrash()
    {
        CurrentTrash = null;
    }

    public void DisposeFellow()
    {
        FellowRobot.transform.parent = FellowRobot.station.transform;
        FellowRobot.transform.localPosition = new Vector3(0, 1.25f, 0);
        FellowRobot.transform.localRotation = Quaternion.identity;

        // FellowRobot.PickedUp = false;
        FellowRobot.disposed = true;
        FellowRobot = null;
        PickedUpObject = null;
    }

    public void ClearFellowRobot()
    {
        FellowRobot = null;
    }

    // formation stuff

    public void TakePosition()
    {
        FellowRobot.formation.AddRobot(this);
    }

    public void BreakFormation()
    {
        Debug.Log(name + ": BreakFormation");

        if (formation != null)
        {
            formation.BreakFormation();
        }
    }

    public void ClearFormation()
    {
        formation = null;
    }

    public override void DestroyObject()
    {
        Destroy(gameObject);
    }

    // collisions stuff

    List<GameObject> ColliderList = new List<GameObject>();

    void OnCollisionEnter(Collision collision)
    {
        ColliderList.Add(collision.gameObject);
    }

    void OnCollisionExit(Collision collision)
    {
        ColliderList.RemoveAll(obj => ReferenceEquals(obj, collision.gameObject));
    }

    public bool ReachedTarget(GameObject target)
    {
        return ColliderList.Contains(target);
    }

    // gizmos stuff

    void OnDrawGizmos()
    {
        if (steering.enabled)
        {
            Quaternion WestRotation = Quaternion.AngleAxis(-FieldOfViewAngle, Vector3.up);
            Quaternion EastRotation = Quaternion.AngleAxis(+FieldOfViewAngle, Vector3.up);
            Vector3 WestDirection = WestRotation * transform.forward;
            Vector3 EastDirection = EastRotation * transform.forward;
            Gizmos.color = RobotColor;
            Gizmos.DrawRay(transform.position, WestDirection * FieldOfViewRange);
            Gizmos.DrawRay(transform.position, EastDirection * FieldOfViewRange);
        }
    }

    Color RobotColor
    {
        get
        {
            switch (name)
            {
                case "RobotYellow":
                    return Color.yellow;
                case "RobotMagenta":
                    return Color.magenta;
                case "RobotCyan":
                    return Color.cyan;
                default:
                    return Color.clear;
            }
        }
    }
}