using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("References")]
    public FixedJoystick joystick;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask nonWalkableLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 120f;
    public float obstacleCheckDistance = 1f;

    [SerializeField] private Rigidbody rb;

    private void FixedUpdate()
    {
        HandleMovement();
        StickToGround();
    }

    private void HandleMovement()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        if (Mathf.Abs(h) > 0.1f)
        {
            float turn = h * rotationSpeed * Time.fixedDeltaTime;
            Quaternion turnRot = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRot);
        }

        if (v > 0.1f && !IsObstacleAhead())
        {
            Vector3 move = transform.forward * v * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
    }

    private void StickToGround()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 1f, groundLayer))
        {
            Vector3 pos = rb.position;
            pos.y = hit.point.y;
            rb.MovePosition(pos);
        }
    }

    private bool IsObstacleAhead()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        float sphereRadius = 0.3f; // Adjust based on player width
        float castDistance = obstacleCheckDistance;

        return Physics.SphereCast(rayOrigin, sphereRadius, transform.forward, out RaycastHit hit, castDistance, nonWalkableLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawRay(rayOrigin, transform.forward * obstacleCheckDistance);
        Gizmos.DrawWireSphere(rayOrigin + transform.forward * obstacleCheckDistance, 0.3f);
    }


}
