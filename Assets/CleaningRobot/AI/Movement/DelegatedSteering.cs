﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DelegatedSteering : MonoBehaviour
{
    public float minLinearSpeed = -2f;
    public float maxLinearSpeed = 5f;
    public float maxAngularSpeed = 10f;

    public MovementStatus status;

    private void Start()
    {
        status = new MovementStatus
        {
            movementDirection = transform.forward
        };
    }

    void FixedUpdate()
    {
        Vector3 totalAcceleration = Vector3.zero;

        foreach (MovementBehaviour mb in GetComponents<MovementBehaviour>())
        {
            totalAcceleration += mb.GetAcceleration(status);
        }

        if (totalAcceleration.magnitude != 0f)
        {
            Vector3 tangentComponent = Vector3.Project(totalAcceleration, status.movementDirection);
            Vector3 normalComponent = totalAcceleration - tangentComponent;

            float tangentAcc = tangentComponent.magnitude * Vector3.Dot(tangentComponent.normalized, status.movementDirection);
            Vector3 right = Quaternion.Euler(0f, 90f, 0f) * status.movementDirection.normalized;
            float rotationAcc = normalComponent.magnitude * Vector3.Dot(normalComponent.normalized, right) * 360f;

            float t = Time.deltaTime;

            float tangentDelta = status.linearSpeed * t + 0.5f * tangentAcc * t * t;
            float rotationDelta = status.angularSpeed * t + 0.5f * rotationAcc * t * t;

            status.linearSpeed += tangentAcc * t;
            status.angularSpeed += rotationAcc * t;

            status.linearSpeed = Mathf.Clamp(status.linearSpeed, minLinearSpeed, maxLinearSpeed);
            status.angularSpeed = Mathf.Clamp(status.angularSpeed, -maxAngularSpeed, maxAngularSpeed);

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.MovePosition(rb.position + transform.forward * tangentDelta);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotationDelta, 0f));

            status.movementDirection = transform.forward;
        }
    }

    private void OnDrawGizmos()
    {
        if (status != null)
        {
            UnityEditor.Handles.Label(transform.position + 2f * transform.up, status.linearSpeed.ToString() + "\n" + status.angularSpeed.ToString());
        }
    }

    public MovementStatus GetStatus()
    {
        return status;
    }
}