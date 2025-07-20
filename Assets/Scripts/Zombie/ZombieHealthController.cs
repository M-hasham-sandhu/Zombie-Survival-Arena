using UnityEngine;

public class ZombieHealthController : MonoBehaviour
{
    [SerializeField] public HealthManager healthManager;

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
        healthManager.ResetHealth();
        ObjectPooler.Instance.ReturnZombie(gameObject);
    }
}
