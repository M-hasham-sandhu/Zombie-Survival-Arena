using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] private HealthManager healthManager;

    private void Awake()
    {
        healthManager.OnDeath += OnPlayerDeath;
    }

    private void OnDestroy()
    {
        if (healthManager != null)
            healthManager.OnDeath -= OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        // Dummy: Destroy player object (replace with game over logic/UI as needed)
        Debug.Log("Player died!");
        Destroy(gameObject);
    }
}
