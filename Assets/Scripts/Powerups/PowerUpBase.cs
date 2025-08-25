using UnityEngine;

public abstract class PowerUpBase : MonoBehaviour
{
    public enum PowerUpType
    {
        Medkit,
        AmmoUpgrade,
        SpeedUpgrade
    }

    public PowerUpType powerUpType;

    public abstract void Apply(GameObject target);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Apply(other.gameObject);
            Destroy(gameObject);
        }
    }
}
