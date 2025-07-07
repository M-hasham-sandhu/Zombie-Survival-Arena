using UnityEngine;


public class ZombieHealthController : MonoBehaviour
{
    [SerializeField] private HealthManager healthManager;

    private void Awake()
    {
        healthManager.OnDeath += OnZombieDeath;
    }

    private void OnDestroy()
    {
        if (healthManager != null)
            healthManager.OnDeath -= OnZombieDeath;
    }

    private void OnZombieDeath()
    {
        // Return zombie to pool
        Debug.Log($"Zombie {name} died, returning to pool.");
        healthManager.ResetHealth();
        ObjectPooler.Instance.ReturnZombie(gameObject);
    }
}
