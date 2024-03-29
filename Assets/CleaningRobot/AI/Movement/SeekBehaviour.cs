﻿using UnityEngine;

public class SeekBehaviour : MovementBehaviour
{
    public Transform destination;
    // public Vector3 position;

    public float gas = 5f;
    public float steer = 30f;
    public float brake = 4f;
    public float brakeAt = 3f;
    public float stopAt = 1f;

    public override Vector3 GetAcceleration(MovementStatus status)
    {
        if (destination != null)
        {
            Vector3 verticalAdj = new Vector3(destination.position.x, transform.position.y, destination.position.z);
            Vector3 toDestination = (verticalAdj - transform.position);

            if (toDestination.magnitude > stopAt)
            {
                Vector3 tangentComponent = Vector3.Project(toDestination.normalized, status.movementDirection);
                Vector3 normalComponent = (toDestination.normalized - tangentComponent);
                return (tangentComponent * (toDestination.magnitude > brakeAt ? gas : -brake)) + (normalComponent * steer);
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
        /*
        if (position != Vector3.positiveInfinity)
        {
            Vector3 verticalAdj = new Vector3(position.x, transform.position.y, position.z);
            Vector3 toDestination = (verticalAdj - transform.position);

            if (toDestination.magnitude > stopAt)
            {
                Vector3 tangentComponent = Vector3.Project(toDestination.normalized, status.movementDirection);
                Vector3 normalComponent = (toDestination.normalized - tangentComponent);
                return (tangentComponent * (toDestination.magnitude > brakeAt ? gas : -brake)) + (normalComponent * steer);
            }
            else
            {
                return Vector3.zero;
            }
        }

        return Vector3.zero;
        */
    }
}
