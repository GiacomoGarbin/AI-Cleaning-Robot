using UnityEngine;

public class WanderBehaviour : MovementBehaviour
{
    public bool wandering;
    private Transform destination;

    public float gas = 3f;
    public float steer = 30f;
    // public float brake = 20f;
    // public float brakeAt = 0.01f;
    // public float stopAt = 0.01f;

    public float WanderOffset = 10.0f;
    public float WanderRadius = 3.0f;
    // public float WanderRate = Mathf.PI / 180;
    public float WanderRate = Mathf.PI * 0.5f;
    // public float WanderRate = 0.25f;
    private float WanderOrientation = 0;
    private float TargetOrientation;
    private Vector3 TargetPosition;
    private GameObject target;

    void Start()
    {
        target = new GameObject("WanderTarget");
        target.AddComponent<MeshFilter>();
        target.AddComponent<MeshRenderer>();
        // target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }

    private Vector3 AsVector(float angle)
    {
        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
    }

    private float RandomBinomial()
    {
        return Random.Range(0.0f, 1.0f) - Random.Range(0.0f, 1.0f);
    }

    private float RandomBinomialReverse()
    {
        float x = RandomBinomial();
        return x - Mathf.Sign(x);
    }

    public void StartWandering()
    {
        wandering = true;
    }

    public void StopWandering()
    {
        wandering = false;
    }

    public override Vector3 GetAcceleration(MovementStatus status)
    {
        if (wandering)
        {
            float sightRange = 5f;
            float sightAngle = 45f;
            int layerMask = ~0;

            bool HitWest = Physics.Raycast(transform.position, Quaternion.Euler(0f, -sightAngle, 0f) * status.movementDirection, sightRange, layerMask);
            bool HitFore = Physics.Raycast(transform.position, status.movementDirection, sightRange, layerMask);
            bool HitEast = Physics.Raycast(transform.position, Quaternion.Euler(0f, +sightAngle, 0f) * status.movementDirection, sightRange, layerMask);

            float factor;

            if (HitWest && !HitFore && !HitEast)
            {
                factor = +0.5f;
            }
            else if (HitWest && HitFore && !HitEast)
            {
                factor = +1;
            }
            else if (HitWest && HitFore && HitEast)
            {
                return Vector3.zero;
            }
            else if (!HitWest && HitFore && HitEast)
            {
                factor = -1;
            }
            else if (!HitWest && !HitFore && HitEast)
            {
                factor = -0.5f;
            }
            else if (!HitWest && HitFore && !HitEast)
            {
                return Vector3.zero;
            }
            else
            {
                // factor = RandomBinomialReverse();
                factor = RandomBinomial();
            }

            // WanderOrientation += Random.Range(-1.0f, +1.0f) * WanderRate;
            // WanderOrientation += RandomBinomial() * WanderRate;
            // WanderOrientation += RandomBinomialReverse() * WanderRate;

            WanderOrientation += factor * WanderRate;
            WanderOrientation = Mathf.Clamp(WanderOrientation, -WanderRate, +WanderRate);
            // Debug.Log(WanderOrientation);

            float CharacterOrientation = Mathf.Atan2(-status.movementDirection.x, +status.movementDirection.z);

            TargetOrientation = WanderOrientation + CharacterOrientation;

            TargetPosition = transform.position + WanderOffset * status.movementDirection;
            TargetPosition += WanderRadius * AsVector(TargetOrientation);

            target.transform.position = TargetPosition;

            destination = target.transform;
        }
        else
        {
            WanderOrientation = 0;
            destination = null;
        }

        if (destination != null)
        {
            Vector3 verticalAdj = new Vector3(destination.position.x, transform.position.y, destination.position.z);
            Vector3 toDestination = (verticalAdj - transform.position);

            Vector3 tangentComponent = Vector3.Project(toDestination.normalized, status.movementDirection);
            Vector3 normalComponent = (toDestination.normalized - tangentComponent);
            return (tangentComponent * gas) + (normalComponent * steer);
        }
        else
        {
            return Vector3.zero;
        }
    }
}