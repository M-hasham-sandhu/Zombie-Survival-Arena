using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] private HealthManager healthManager;
    private Animator animator;

    [SerializeField] private float deathDelay = 1.5f;

    public HealthManager HealthManager => healthManager;

    private void Awake()
    {
        healthManager.OnDeath += OnPlayerDeath;
        animator = GetComponent<Animator>();
    }

    private void OnPlayerDeath()
    {
        // Play death sound effect
        SoundManager.Instance?.PlaySFX("PlayerDeath");

        if (animator != null)
        {
            animator.ResetTrigger("walk");
            animator.ResetTrigger("idle");
            animator.SetTrigger("die");
        }

        var movement = GetComponent<CharacterMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence()
    {
        yield return new WaitForSeconds(deathDelay);

        // Stop all sounds before scene transition
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopAllSFX();
        }

        Destroy(gameObject);
        
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        if (healthManager != null)
            healthManager.OnDeath -= OnPlayerDeath;
    }
}
