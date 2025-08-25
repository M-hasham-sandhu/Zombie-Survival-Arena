using UnityEngine;

public class SpeedUpgradePowerUp : PowerUpBase
{
    private void Awake() { powerUpType = PowerUpType.SpeedUpgrade; }

    public override void Apply(GameObject target)
    {
        var movement = target.GetComponent<CharacterMovement>();
        if (movement != null)
        {
            movement.moveSpeed *= 1.1f;
        }
    }
} 