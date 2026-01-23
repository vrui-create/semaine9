using UnityEngine;
using Game.Character;

[DisallowMultipleComponent]
public class OrbFollow : MonoBehaviour, IAIBehaviour
{
    [Header("Références joueur")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CharacterManager playerManager;
    [SerializeField] private string playerTag = "Player";

    [Header("Détection & poursuite")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float loseSightRadius = 8f;
    [SerializeField] private float chaseSpeedMultiplier = 1.2f;


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
    }

    public void Tick()
    {
        if (_controller == null) return;

        switch (_controller.CurrentState)
        {
            case AIState.Idle:
                HandleIdle();
                break;
            case AIState.Chase:
                HandleChase();
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
            _controller.ChangeState(AIState.Idle);
            return;
        }

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist > loseSightRadius)
        {
            _controller.ChangeState(AIState.Idle);
            return;
        }

        Vector3 toPlayer = playerTransform.position - transform.position;
        toPlayer.y = 0f;
        Vector3 dir = toPlayer.normalized;
        Vector2 input = new Vector2(dir.x, dir.z);
        _movement.SetMoveInput(input);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
#endif
}
