using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CrosshairUIManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Crosshair Reference")]
    public Image crosshairImage;
    [Header("Shoot Button Reference")]
    public GameObject shootButton; // Assign your UI Button here
    [Header("Shoot Manager Reference")]
    public ShootManager shootManager;

    private void Awake()
    {
        if (crosshairImage == null)
        {
            Debug.LogError("CrosshairUIManager: Crosshair Image not assigned!");
            enabled = false;
        }
        if (shootButton != null)
        {
            // Add this script as an event handler for the button
            EventTrigger trigger = shootButton.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = shootButton.AddComponent<EventTrigger>();

            // Pointer Down
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((eventData) => { OnPointerDown((PointerEventData)eventData); });
            trigger.triggers.Add(entryDown);

            // Pointer Up
            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((eventData) => { OnPointerUp((PointerEventData)eventData); });
            trigger.triggers.Add(entryUp);
        }
    }

    public void SetVisible(bool visible)
    {
        if (crosshairImage != null)
            crosshairImage.enabled = visible;
    }

    public void SetColor(Color color)
    {
        if (crosshairImage != null)
            crosshairImage.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("CrosshairUIManager: Pointer Down - Attempting to start shooting");
        if (shootManager != null)
        {
            Debug.Log("CrosshairUIManager: shootManager is assigned, calling StartShooting()");
            shootManager.StartShooting();
        }
        else
        {
            Debug.LogWarning("CrosshairUIManager: shootManager is NOT assigned!");
        }
        SetVisible(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("CrosshairUIManager: Pointer Up - Attempting to stop shooting");
        if (shootManager != null)
        {
            Debug.Log("CrosshairUIManager: shootManager is assigned, calling StopShooting()");
            shootManager.StopShooting();
        }
        else
        {
            Debug.LogWarning("CrosshairUIManager: shootManager is NOT assigned!");
        }
        SetVisible(false); // Optionally hide crosshair when not shooting
    }
}
