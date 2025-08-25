using UnityEngine;

using Cinemachine;

public class CameraAiming : MonoBehaviour
{
    [Header("Aiming Settings")]
    public float sensitivity = 0.2f;
    public float minY = -30f;
    public float maxY = 60f;
    [Tooltip("Reference to the Cinemachine virtual camera.")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("Player Reference")]
    [Tooltip("Reference to the player CharacterMovement script.")]
    [SerializeField] private CharacterMovement playerCharacter;

    [Header("Crosshair Reference")]
    [Tooltip("Reference to the crosshair UI manager.")]
    [SerializeField] private CrosshairUIManager crosshairUI;

    private float rotationY = 0f;
    private float rotationX = 0f;
    private Vector2 lastTouchPosition;
    private bool isAiming = false;

    private Transform cameraTarget;

    private void Start()
    {
        if (virtualCamera == null)
        {
            enabled = false;
            return;
        }
        cameraTarget = virtualCamera.Follow != null ? virtualCamera.Follow : virtualCamera.transform;
        Vector3 angles = cameraTarget.eulerAngles;
        rotationY = angles.y;
        rotationX = angles.x;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                // Only listen to touches on the right half of the screen
                if (touch.position.x < Screen.width / 2f) continue;

                if (touch.phase == TouchPhase.Began)
                {
                    lastTouchPosition = touch.position;
                    isAiming = true;
                    if (crosshairUI != null) crosshairUI.SetVisible(true);
                }
                else if (touch.phase == TouchPhase.Moved && isAiming)
                {
                    Vector2 delta = touch.position - lastTouchPosition;
                    lastTouchPosition = touch.position;
                    rotationY += delta.x * sensitivity;
                    rotationX -= delta.y * sensitivity;
                    rotationX = Mathf.Clamp(rotationX, minY, maxY);
                    cameraTarget.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
                    // Align player to camera Y after aiming
                    if (playerCharacter != null)
                    {
                        playerCharacter.AlignToCameraY(cameraTarget);
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isAiming = false;
                    if (crosshairUI != null) crosshairUI.SetVisible(false);
                }
            }
        }
        else
        {
            if (crosshairUI != null) crosshairUI.SetVisible(false);
        }
    }
}
