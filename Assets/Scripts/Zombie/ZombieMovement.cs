using UnityEngine;
using UnityEngine.AI;

public enum ZombieState
{
    Idle,
    Chase,
    Attack,
    Die
}

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Death Settings")]
    [SerializeField] private float deathDuration = 3f; // How long to stay in death state

    public ZombieState currentState = ZombieState.Idle;

    private float deathTimer = 0f;
    private bool isDead = false;

    // Animation parameter names
    private const string WALK_TRIGGER = "Walk";
    private const string DIE_TRIGGER = "Die";
    private const string ATTACK_TRIGGER = "Attack";

    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private ZombieState previousState = ZombieState.Idle;

    private void Update()
    {
        if (isDead)
        {
            HandleDeathState();
            return;
        }

        // Only play animation when state changes
        if (currentState != previousState)
        {
            switch (currentState)
            {
                case ZombieState.Idle:
                    PlayIdleAnimation();
                    break;
                case ZombieState.Chase:
                    PlayMovementAnimation();
                    break;
                case ZombieState.Attack:
                    PlayAttackAnimation();
                    break;
                case ZombieState.Die:
                    PlayDieAnimation();
                    break;
            }
            previousState = currentState;
        }

        DetectPlayer();
        HandleMovement();
    }

    private void HandleDeathState()
    {
        deathTimer += Time.deltaTime;
        if (deathTimer >= deathDuration)
        {
            // Return from death state
            isDead = false;
            deathTimer = 0f;
            currentState = ZombieState.Idle;
            ResetAnimator();
        }
    }

    private void PlayIdleAnimation()
    {
        // Idle is the default state, no trigger needed
        // Don't reset animator here as it can interfere with movement
    }

    private void PlayMovementAnimation()
    {
        animator.SetTrigger(WALK_TRIGGER);
    }

    private void PlayAttackAnimation()
    {
        animator.SetTrigger(ATTACK_TRIGGER);

        // Try to damage the player when attacking
        if (target != null)
        {
            var playerHealth = target.GetComponent<PlayerHealthController>();
            if (playerHealth != null && playerHealth.HealthManager != null)
            {
                playerHealth.HealthManager.TakeDamage(10f);
                Debug.Log("Dealing Damage to Player: 10");
            }
            else
            {
                Debug.Log("PlayerHealthController or HealthManager not found on target.");
            }

        }
        else
        {
            Debug.LogWarning("Target is null, cannot attack.");
        }
    }

    private void PlayDieAnimation()
    {
        animator.SetTrigger(DIE_TRIGGER);
        isDead = true;
        deathTimer = 0f;
    }

    private void ResetAnimator()
    {
        // Reset all triggers to return to idle state
        animator.ResetTrigger(WALK_TRIGGER);
        animator.ResetTrigger(ATTACK_TRIGGER);
        animator.ResetTrigger(DIE_TRIGGER);
    }

    private void ResetMovementTriggers()
    {
        // Only reset movement triggers, not attack or die
        animator.ResetTrigger(WALK_TRIGGER);
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            target = hits[0].transform;
            if (currentState == ZombieState.Idle)
            {
                currentState = ZombieState.Chase;
            }
        }
        else
        {
            if (currentState == ZombieState.Chase)
            {
                currentState = ZombieState.Idle;
                ResetMovementTriggers(); // Reset movement triggers when returning to idle
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null && currentState == ZombieState.Idle)
        {
            currentState = ZombieState.Chase;
        }
    }

    public void TriggerDeath()
    {
        currentState = ZombieState.Die;
    }

    private void HandleMovement()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            
            // Handle both Chase and Attack states
            if (currentState == ZombieState.Chase || currentState == ZombieState.Attack)
            {
                if (distance > attackRange)
                {
                    // Resume chasing if player moves away during attack
                    if (currentState == ZombieState.Attack)
                    {
                        currentState = ZombieState.Chase;
                    }
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                }
                else
                {
                    agent.isStopped = true;
                    currentState = ZombieState.Attack;
                }
            }
        }
        else
        {
            agent.ResetPath();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
