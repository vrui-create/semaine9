using UnityEngine;
using Game.Character;

[DisallowMultipleComponent]
public class EnemyAI : MonoBehaviour, IAIBehaviour
{
    [Header("Références joueur")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CharacterManager playerManager;
    [SerializeField] private string playerTag = "Player";

    [Header("Patrouille")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeedMultiplier = 1f;
    [SerializeField] private float waypointReachDistance = 0.3f;
    [SerializeField] private float waitAtPoint = 1f;
    [SerializeField] private bool loopPatrol = true;

    [Header("Détection & poursuite")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float loseSightRadius = 8f;
    [SerializeField] private float chaseSpeedMultiplier = 1.2f;

    [Header("Attaque")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack";

    private AIController _controller;
    private EnemyMovement _movement;

    private int _currentPatrolIndex;
    private float _waitTimer;
    private float _attackTimer;

    public void Initialize(object controller)
    {
        _controller = controller as AIController;
        if (_controller == null)
        {
            Debug.LogError("EnemyAI nécessite un AIController valide.", this);
            enabled = false;
            return;
        }

        _movement = _controller.Movement;

        if (animator == null)
            animator = GetComponent<Animator>();

        // Trouve automatiquement le joueur si non assigné
        if (playerTransform == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                playerManager = playerObj.GetComponent<CharacterManager>();
            }
        }

        _controller.ChangeState(patrolPoints != null && patrolPoints.Length > 0 ? AIState.Patrol : AIState.Idle);
    }

    public void Tick()
    {
        if (_controller == null) return;

        switch (_controller.CurrentState)
        {
            case AIState.Idle:
                HandleIdle();
                break;
            case AIState.Patrol:
                HandlePatrol();
                break;
            case AIState.Chase:
                HandleChase();
                break;
            case AIState.Attack:
                HandleAttack();
                break;
        }

        if (_controller.CurrentState == AIState.Patrol || _controller.CurrentState == AIState.Idle)
        {
            TryDetectPlayer();
        }
    }

    public void FixedTick()
    {
        // Mouvement géré par CharacterMovement.FixedUpdate
    }

    public void OnStateChanged(int oldState, int newState)
    {
        if (_movement == null) return;

        var newEnum = (AIState)newState;
        switch (newEnum)
        {
            case AIState.Patrol:
                _movement.SetSpeedModifier(patrolSpeedMultiplier);
                break;
            case AIState.Chase:
                _movement.SetSpeedModifier(chaseSpeedMultiplier);
                break;
            default:
                _movement.SetSpeedModifier(1f);
                break;
        }
    }

    private void HandleIdle()
    {
        if (_movement != null)
            _movement.SetMoveInput(Vector2.zero);
        _movement.IsWalking(false);
    }

    private void HandlePatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            _controller.ChangeState(AIState.Idle);
            return;
        }

        Transform target = patrolPoints[_currentPatrolIndex];
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude <= waypointReachDistance)
        {
            _movement.SetMoveInput(Vector2.zero);
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= waitAtPoint)
            {
                _waitTimer = 0f;
                _currentPatrolIndex++;
                if (_currentPatrolIndex >= patrolPoints.Length)
                {
                    _currentPatrolIndex = loopPatrol ? 0 : patrolPoints.Length - 1;
                }
            }
            return;
        }

        Vector3 dir = toTarget.normalized;
        Vector2 input = new Vector2(dir.x, dir.z);
        _movement.SetMoveInput(input);
        _movement.IsWalking(false);
    }

    private void TryDetectPlayer()
    {
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist <= detectionRadius)
        {
            _controller.ChangeState(AIState.Chase);
        }
    }

    private void HandleChase()
    {
        if (playerTransform == null)
        {
            _controller.ChangeState(AIState.Patrol);
            return;
        }

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist > loseSightRadius)
        {
            _controller.ChangeState(AIState.Patrol);
            return;
        }

        if (dist <= attackRange)
        {
            _controller.ChangeState(AIState.Attack);
            return;
        }

        Vector3 toPlayer = playerTransform.position - transform.position;
        toPlayer.y = 0f;
        Vector3 dir = toPlayer.normalized;
        Vector2 input = new Vector2(dir.x, dir.z);
        _movement.SetMoveInput(input);
        _movement.IsWalking(true);
    }

    private void HandleAttack()
    {
        if (playerTransform == null)
        {
            _controller.ChangeState(AIState.Patrol);
            return;
        }

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > attackRange)
        {
            _controller.ChangeState(AIState.Chase);
            return;
        }

        _movement.SetMoveInput(Vector2.zero);
        _movement.IsWalking(false);

        _attackTimer -= Time.deltaTime;
        if (_attackTimer > 0f) return;

        _attackTimer = attackCooldown;

        if (animator != null && !string.IsNullOrEmpty(attackTriggerName))
        {
            animator.SetTrigger(attackTriggerName);
        }

        if (playerManager != null)
        {
            playerManager.TakeDamage((int)attackDamage);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}
