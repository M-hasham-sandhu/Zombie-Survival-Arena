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

    public ZombieState currentState = ZombieState.Idle;

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

    private void Update()
    {
        switch (currentState)
        {
            case ZombieState.Idle:
                // TODO: Trigger idle animation
                break;
            case ZombieState.Chase:
                // TODO: Trigger chase animation
                break;
            case ZombieState.Attack:
                // TODO: Trigger attack animation
                break;
            case ZombieState.Die:
                // TODO: Trigger die animation
                break;
        }
        DetectPlayer();
        HandleMovement();
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
        // else: do not clear target if already set externally (e.g., by spawner)
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void HandleMovement()
    {
        if (target != null)
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
                // Attack logic will go here later
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
