using UnityEngine;

public class SlotArriveBehaviour : MovementBehaviour
{
    // public Vector3 TargetPosition;
    public Transform TargetPosition;

    public float GoalRadius = 1;
    public float SlowRadius = 2;

    public float MaxSpeed = 5;
    public float MaxAcceleration = 100;

    public float TimeToTarget = 0.1f;

    public override Vector3 GetAcceleration(MovementStatus status)
    {
        if (TargetPosition != null)
        {
            // TargetPosition = new Vector3(TargetPosition.x, transform.position.y, TargetPosition.z);
            Vector3 TargetPosition = new Vector3(this.TargetPosition.position.x, transform.position.y, this.TargetPosition.position.z);
            Vector3 TargetDirection = TargetPosition - transform.position;

            float TargetDistance = TargetDirection.magnitude;

            if (TargetDistance > GoalRadius)
            {
                float speed;

                if (TargetDistance > SlowRadius)
                {
                    speed = MaxSpeed;
                }
                else
                {
                    speed = MaxSpeed * TargetDistance / SlowRadius;
                }

                Vector3 TargetVelocity = speed * TargetDirection.normalized;
                Vector3 CharacterVelocity = status.linearSpeed * status.movementDirection;

                Vector3 LinearAcceleration = TargetVelocity - CharacterVelocity;
                LinearAcceleration /= TimeToTarget;

                if (LinearAcceleration.magnitude > MaxAcceleration)
                {
                    LinearAcceleration = MaxAcceleration * LinearAcceleration.normalized;
                }

                return LinearAcceleration;
            }
            else
            {
                return Vector3.zero;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }
}
