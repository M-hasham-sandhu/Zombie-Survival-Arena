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

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private bool isCrawler = false; // Randomly assigned - some zombies only crawl

    [Header("Death Settings")]
    [SerializeField] private float deathDuration = 3f; // How long to stay in death state

    public ZombieState currentState = ZombieState.Idle;

    private float deathTimer = 0f;
    private bool isDead = false;

    // Animation parameter names
    private const string WALK_TRIGGER = "Walk";
    private const string CRAWL_TRIGGER = "Crawl";
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

        // Randomly assign whether this zombie walks or crawls
        isCrawler = Random.Range(0, 2) == 1; // 50% chance to be a crawler
        
        // Adjust NavMeshAgent properties for crawling zombies
        if (isCrawler)
        {
            AdjustForCrawling();
        }
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
        if (isCrawler)
        {
            animator.SetTrigger(CRAWL_TRIGGER);
        }
        else
        {
            animator.SetTrigger(WALK_TRIGGER);
        }
    }

    private void PlayAttackAnimation()
    {
        animator.SetTrigger(ATTACK_TRIGGER);
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
        animator.ResetTrigger(CRAWL_TRIGGER);
        animator.ResetTrigger(ATTACK_TRIGGER);
        animator.ResetTrigger(DIE_TRIGGER);
    }

    private void ResetMovementTriggers()
    {
        // Only reset movement triggers, not attack or die
        animator.ResetTrigger(WALK_TRIGGER);
        animator.ResetTrigger(CRAWL_TRIGGER);
    }

    private void AdjustForCrawling()
    {
        if (agent != null)
        {
            // Reduce height for crawling zombies
            agent.height = 0.5f; // Adjust this value based on your zombie model
            agent.baseOffset = 0.25f; // Adjust this to keep zombie close to ground
            agent.radius = 0.3f; // Smaller radius for crawling
            agent.speed = 2f; // Slower speed for crawling zombies
        }
    }

    private void AdjustForWalking()
    {
        if (agent != null)
        {
            // Reset to normal walking values
            agent.height = 2f; // Adjust this value based on your zombie model
            agent.baseOffset = 1f; // Normal height offset
            agent.radius = 0.5f; // Normal radius
            agent.speed = 3.5f; // Normal walking speed
        }
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

    public void SetCrawler(bool crawler)
    {
        isCrawler = crawler;
        if (isCrawler)
        {
            AdjustForCrawling();
        }
        else
        {
            AdjustForWalking();
        }
    }

    public void TriggerDeath()
    {
        currentState = ZombieState.Die;
    }

    private void HandleMovement()
    {
        if (target != null && currentState == ZombieState.Chase)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else
            {
                agent.isStopped = true;
                currentState = ZombieState.Attack;
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
