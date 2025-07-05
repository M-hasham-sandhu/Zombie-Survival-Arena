using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Joystick Reference")]
    public FixedJoystick joystick;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 120f; // degrees per second

    [SerializeField] private Rigidbody rb;

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        // Turning (Tank-style): Always allow rotation left/right
        if (Mathf.Abs(h) > 0.1f)
        {
            float turn = h * rotationSpeed * Time.fixedDeltaTime;
            Quaternion turnRot = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRot);
        }

        // Forward movement only
        if (v > 0.1f)
        {
            Vector3 move = transform.forward * v * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
    }
}
