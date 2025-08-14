using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    
    [Header("References")]
    public FixedJoystick joystick;
    [SerializeField] private LayerMask nonWalkableLayer;
    [SerializeField] private Animator animator;

     [SerializeField] private Rigidbody rb;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 120f;
    public float obstacleCheckDistance = 1f;

    [Header("Gun Settings")]
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Vector3 gunWalkRotation = new Vector3(220f, 134f, 199.3f);
    [SerializeField] private Vector3 gunIdleRotation = Vector3.zero;
    [SerializeField] private Vector3 gunShootRotation = new Vector3(224.13f, -62.67999f, 259.022f); // Adjust these values
    [SerializeField] private float gunRotationSpeed = 8f; // Smoothing speed for rotation

    [Header("UI References")]
    [SerializeField] private UnityEngine.UI.Button shootButton; // Add this at the top with other serialized fields

    private bool isShooting = false;


    private void Awake()
    {
       
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        // Debug: Press D to trigger die animation
        if (Input.GetKeyDown(KeyCode.D) && animator != null)
        {
            animator.ResetTrigger("walk");
            animator.ResetTrigger("idle");
            animator.SetTrigger("die");
        }
    }

    private void HandleMovement()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        bool isMoving = false;

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
            isMoving = true;
        }

        // Update shoot button visibility based on movement
        if (shootButton != null)
        {
            shootButton.gameObject.SetActive(isMoving);
        }

        // Handle gun rotation
        if (gunTransform != null)
        {
            Vector3 targetRotation;
            if (isShooting)
            {
                targetRotation = gunShootRotation;
            }
            else
            {
                targetRotation = isMoving ? gunWalkRotation : gunIdleRotation;
            }

            gunTransform.localRotation = Quaternion.Lerp(
                gunTransform.localRotation,
                Quaternion.Euler(targetRotation),
                Time.deltaTime * gunRotationSpeed
            );
        }

        if (animator != null)
        {
            if (isMoving)
            {
                animator.ResetTrigger("idle");
                animator.SetTrigger("walk");
            }
            else
            {
                animator.ResetTrigger("walk");
                animator.SetTrigger("idle");
            }
        }
    }

    private bool IsObstacleAhead()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        float sphereRadius = 0.3f; 
        float castDistance = obstacleCheckDistance;

        return Physics.SphereCast(rayOrigin, sphereRadius, transform.forward, out RaycastHit hit, castDistance, nonWalkableLayer);
    }

    public void AlignToCameraY(Transform cameraTransform, float alignSpeed = 720f)
    {
        if (cameraTransform == null) return;
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        if (camForward.sqrMagnitude < 0.01f) return;
        Quaternion targetRot = Quaternion.LookRotation(camForward);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, alignSpeed * Time.fixedDeltaTime));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawRay(rayOrigin, transform.forward * obstacleCheckDistance);
        Gizmos.DrawWireSphere(rayOrigin + transform.forward * obstacleCheckDistance, 0.3f);
    }

    public void PlayWalkAnimation()
    {
        if (animator != null)
        {
            animator.ResetTrigger("idle");
            animator.SetTrigger("walk");
        }
    }

    public void PlayIdleAnimation()
    {
        if (animator != null)
        {
            animator.ResetTrigger("walk");
            animator.SetTrigger("idle");
        }
    }

    // Add this method to be called from ShootManager
    public void SetShootingState(bool shooting)
    {
        isShooting = shooting;
    }
}
