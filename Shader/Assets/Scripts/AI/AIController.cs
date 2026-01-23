using UnityEngine;
using Game.Character;

public enum AIType
{
    None,
    Enemy,
    Orb,
    NPC
}

public enum AIState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Dialogue,
    Disabled
}

[RequireComponent(typeof(EnemyMovement))]
[DisallowMultipleComponent]
public class AIController : MonoBehaviour
{
    [Header("Type d'IA")]
    [SerializeField] private AIType aiType = AIType.Enemy;

    [Header("Références")]
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private CharacterManager characterManager;

    private IAIBehaviour _currentBehaviour;
    private AIState _currentState = AIState.Idle;

    public AIType Type => aiType;
    public AIState CurrentState => _currentState;
    public EnemyMovement Movement => enemyMovement;
    public CharacterManager Manager => characterManager;

    private void Awake()
    {
        if (enemyMovement == null)
            enemyMovement = GetComponent<EnemyMovement>();

        if (characterManager == null)
            characterManager = GetComponent<CharacterManager>();
    }

    private void OnEnable()
    {
        SetupBehaviour();
    }

    private void Update()
    {
        _currentBehaviour?.Tick();
    }

    private void FixedUpdate()
    {
        _currentBehaviour?.FixedTick();
    }

    public void ChangeState(AIState newState)
    {
        if (_currentState == newState)
            return;

        var oldState = _currentState;
        _currentState = newState;
        _currentBehaviour?.OnStateChanged((int)oldState, (int)newState);
    }

    private void SetupBehaviour()
    {
        // Désactiver d'abord tout comportement existant
        var enemy = GetComponent<EnemyAI>();
        var npc = GetComponent<NPCAI>();
        var orbFollow = GetComponent<OrbFollow>();

        if (enemy != null) enemy.enabled = false;
        if (npc != null) npc.enabled = false;

        switch (aiType)
        {
            case AIType.Enemy:
                if (enemy == null) enemy = gameObject.AddComponent<EnemyAI>();
                _currentBehaviour = enemy;
                enemy.enabled = true;
                break;

            case AIType.NPC:
                if (npc == null) npc = gameObject.AddComponent<NPCAI>();
                _currentBehaviour = npc;
                npc.enabled = true;
                break;

            case AIType.Orb:
                if (orbFollow == null) orbFollow = gameObject.AddComponent<OrbFollow>();
                _currentBehaviour = orbFollow;
                orbFollow.enabled = true;
                break;

            default:
                _currentBehaviour = null;
                break;
        }

        _currentBehaviour?.Initialize(this);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (enemyMovement == null)
            enemyMovement = GetComponent<EnemyMovement>();
        if (characterManager == null)
            characterManager = GetComponent<CharacterManager>();
    }
#endif
}
