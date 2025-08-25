using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public System.Action<float, float> OnHealthChanged; // (current, max)
    public System.Action OnDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        IsDead = false;
    }

    /// <summary>
    /// Apply damage to this entity.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Heal this entity.
    /// </summary>
    public void Heal(float amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    /// <summary>
    /// Instantly kill this entity.
    /// </summary>
    public void Kill()
    {
        if (!IsDead)
        {
            CurrentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;
        OnDeath?.Invoke();
        // Optionally: disable components, play animation, etc.
    }

    /// <summary>
    /// Reset health to max and revive (for pooling).
    /// </summary>
    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        IsDead = false;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}
