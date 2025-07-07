using UnityEngine;
using UnityEngine.UI;

public class CrosshairUIManager : MonoBehaviour
{
    [Header("Crosshair Reference")]
    public Image crosshairImage;

    private void Awake()
    {
        if (crosshairImage == null)
        {
            Debug.LogError("CrosshairUIManager: Crosshair Image not assigned!");
            enabled = false;
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
}
