using UnityEngine;

public class MedkitPowerUp : PowerUpBase
{
    private void Awake() { powerUpType = PowerUpType.Medkit; }

    public override void Apply(GameObject target)
    {
        var playerHealth = target.GetComponent<PlayerHealthController>();
        if (playerHealth != null && playerHealth.HealthManager != null)
            playerHealth.HealthManager.Heal(25f);
    }
} 