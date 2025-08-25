using UnityEngine;

public class AmmoUpgradePowerUp : PowerUpBase
{
    private void Awake() { powerUpType = PowerUpType.AmmoUpgrade; }

    public override void Apply(GameObject target)
    {
        var shootManager = target.GetComponent<ShootManager>();
        if (shootManager != null)
        {
            shootManager.bulletDamage *= 1.1f;
        }
    }
} 