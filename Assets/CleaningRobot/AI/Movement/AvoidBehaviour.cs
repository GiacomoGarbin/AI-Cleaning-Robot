using UnityEngine;

public class AvoidBehaviour : MovementBehaviour
{
    public float sightRange = 5f;
    public float sightAngle = 45f;

    // public float steer = 15f;
    // public float backpedal = 10f;
    public float steer = 30f;
    public float backpedal = 20f;

    public int LayerMask = ~0;

    MovementStatus status;

    public override Vector3 GetAcceleration(MovementStatus status)
    {
        // layerMask = 1 << 8;
        // layerMask = ~layerMask;

        this.status = status;

        bool HitWest = Physics.Raycast(transform.position, Quaternion.Euler(0f, -sightAngle, 0f) * status.movementDirection, sightRange, LayerMask);
        bool HitFore = Physics.Raycast(transform.position, status.movementDirection, sightRange, LayerMask);
        bool HitEast = Physics.Raycast(transform.position, Quaternion.Euler(0f, +sightAngle, 0f) * status.movementDirection, sightRange, LayerMask);

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

    private Vector3 ObjectSize(GameObject go)
    {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
        foreach (Collider c in go.GetComponentsInChildren<Collider>())
        {
            b.Encapsulate(c.bounds);
        }
        return b.size;
    }

    void OnDrawGizmos()
    {
        bool HitWest = Physics.Raycast(transform.position, Quaternion.Euler(0f, -sightAngle, 0f) * status.movementDirection, sightRange, LayerMask);
        bool HitFore = Physics.Raycast(transform.position, status.movementDirection, sightRange, LayerMask);
        bool HitEast = Physics.Raycast(transform.position, Quaternion.Euler(0f, +sightAngle, 0f) * status.movementDirection, sightRange, LayerMask);

        Debug.DrawRay(transform.position, Quaternion.Euler(0f, -sightAngle, 0f) * status.movementDirection * sightRange, HitWest ? Color.red : Color.white);
        Debug.DrawRay(transform.position, status.movementDirection * sightRange, HitFore ? Color.red : Color.white);
        Debug.DrawRay(transform.position, Quaternion.Euler(0f, +sightAngle, 0f) * status.movementDirection * sightRange, HitEast ? Color.red : Color.white);
    }
}