using UnityEngine;

public class SlotAvoidBehaviour : MovementBehaviour
{
    public int SlotIndex;

    public float FieldOfViewAngle = 45;
    public float FieldOfViewRange = 5;
    public float FieldOfViewScale = 0.5f;

    public float steer = 30f;
    public float backpedal = 20f;

    public int LayerMask;

    void Awake()
    {
        LayerMask = ~(1 << UnityEngine.LayerMask.NameToLayer("TrashCompactor"));
    }

    public override Vector3 GetAcceleration(MovementStatus status)
    {
        /*
        bool HitWest = Physics.Raycast(transform.position, Quaternion.Euler(0, -FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange, LayerMask);
        bool HitFore = Physics.Raycast(transform.position, status.movementDirection, FieldOfViewRange, LayerMask);
        bool HitEast = Physics.Raycast(transform.position, Quaternion.Euler(0, +FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange, LayerMask);
        */

        bool HitWest, HitFore, HitEast;

        switch (SlotIndex)
        {
            case 2:
                HitWest = Physics.Raycast(transform.position, Quaternion.Euler(0, -FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange * FieldOfViewScale, LayerMask);
                HitFore = Physics.Raycast(transform.position, status.movementDirection, FieldOfViewRange * FieldOfViewScale, LayerMask);
                HitEast = Physics.Raycast(transform.position, Quaternion.Euler(0, +FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange, LayerMask);
                break;
            case 1:
                HitWest = Physics.Raycast(transform.position, Quaternion.Euler(0, -FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange, LayerMask);
                HitFore = Physics.Raycast(transform.position, status.movementDirection, FieldOfViewRange * FieldOfViewScale, LayerMask);
                HitEast = Physics.Raycast(transform.position, Quaternion.Euler(0, +FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange * FieldOfViewScale, LayerMask);
                break;
            default:
                HitWest = Physics.Raycast(transform.position, Quaternion.Euler(0, -FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange, LayerMask);
                HitFore = Physics.Raycast(transform.position, status.movementDirection, FieldOfViewRange, LayerMask);
                HitEast = Physics.Raycast(transform.position, Quaternion.Euler(0, +FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange, LayerMask);
                break;
        }

        Vector3 right = Quaternion.Euler(0f, 90f, 0f) * status.movementDirection.normalized;

        if (HitWest && !HitFore && !HitEast)
        {
            return right * steer;
        }
        else if (HitWest && HitFore && !HitEast)
        {
            return right * steer * 2f;
        }
        else if (HitWest && HitFore && HitEast)
        {
            return -status.movementDirection.normalized * backpedal;
        }
        else if (!HitWest && HitFore && HitEast)
        {
            return -right * steer * 2f;
        }
        else if (!HitWest && !HitFore && HitEast)
        {
            return -right * steer;
        }
        else if (!HitWest && HitFore && !HitEast)
        {
            return right * steer;
        }

        return Vector3.zero;
    }

    void OnDrawGizmos()
    {
        MovementStatus status = GetComponent<DelegatedSteering>().status;

        bool HitWest, HitFore, HitEast;

        switch (SlotIndex)
        {
            case 2:
                HitWest = Physics.Raycast(transform.position, Quaternion.Euler(0, -FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange * FieldOfViewScale, LayerMask);
                HitFore = Physics.Raycast(transform.position, status.movementDirection, FieldOfViewRange * FieldOfViewScale, LayerMask);
                HitEast = Physics.Raycast(transform.position, Quaternion.Euler(0, +FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange, LayerMask);
                break;
            case 1:
                HitWest = Physics.Raycast(transform.position, Quaternion.Euler(0, -FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange, LayerMask);
                HitFore = Physics.Raycast(transform.position, status.movementDirection, FieldOfViewRange * FieldOfViewScale, LayerMask);
                HitEast = Physics.Raycast(transform.position, Quaternion.Euler(0, +FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange * FieldOfViewScale, LayerMask);
                break;
            default:
                HitWest = Physics.Raycast(transform.position, Quaternion.Euler(0, -FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange, LayerMask);
                HitFore = Physics.Raycast(transform.position, status.movementDirection, FieldOfViewRange, LayerMask);
                HitEast = Physics.Raycast(transform.position, Quaternion.Euler(0, +FieldOfViewAngle, 0) * status.movementDirection, FieldOfViewRange, LayerMask);
                break;
        }

        switch (SlotIndex)
        {
            case 2:
                Debug.DrawRay(transform.position, Quaternion.Euler(0, -FieldOfViewAngle, 0) * status.movementDirection * FieldOfViewRange * FieldOfViewScale, HitWest ? Color.red : Color.white);
                Debug.DrawRay(transform.position, status.movementDirection * FieldOfViewRange * FieldOfViewScale, HitFore ? Color.red : Color.white);
                Debug.DrawRay(transform.position, Quaternion.Euler(0, +FieldOfViewAngle, 0) * status.movementDirection * FieldOfViewRange, HitEast ? Color.red : Color.white);
                break;
            case 1:
                Debug.DrawRay(transform.position, Quaternion.Euler(0, -FieldOfViewAngle, 0) * status.movementDirection * FieldOfViewRange, HitWest ? Color.red : Color.white);
                Debug.DrawRay(transform.position, status.movementDirection * FieldOfViewRange * FieldOfViewScale, HitFore ? Color.red : Color.white);
                Debug.DrawRay(transform.position, Quaternion.Euler(0, +FieldOfViewAngle, 0) * status.movementDirection * FieldOfViewRange * FieldOfViewScale, HitEast ? Color.red : Color.white);
                break;
            default:
                Debug.DrawRay(transform.position, Quaternion.Euler(0, -FieldOfViewAngle, 0) * status.movementDirection * FieldOfViewRange, HitWest ? Color.red : Color.white);
                Debug.DrawRay(transform.position, status.movementDirection * FieldOfViewRange, HitFore ? Color.red : Color.white);
                Debug.DrawRay(transform.position, Quaternion.Euler(0, +FieldOfViewAngle, 0) * status.movementDirection * FieldOfViewRange, HitEast ? Color.red : Color.white);
                break;
        }
    }
}