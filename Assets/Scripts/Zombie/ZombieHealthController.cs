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
        healthManager.ResetHealth();
        ObjectPooler.Instance.ReturnZombie(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            healthManager.TakeDamage(50);
        }
    }
}
