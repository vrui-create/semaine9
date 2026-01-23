using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private HealthSystem _healthSystem;
    private Stats _stats;
    public float health;
    [SerializeField] Animator animator;

    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private int minOrbs = 1;
    [SerializeField] private int maxOrbs = 5;
    [SerializeField] private float dropRadius = 1f;
    [SerializeField] private float dropHeightOffset = 0.2f;
    [SerializeField] private float minDistanceBetweenOrbs = 0.4f;

    private int baseLayer;
    private int hitLayer;


    private void Awake()
    {
        _stats = GetComponent<Stats>();
        if (_stats == null)
        {
            Debug.LogError("Stats manquant sur " + gameObject.name);
            _stats = gameObject.AddComponent<Stats>();
        }

        _healthSystem = new HealthSystem(_stats._maxHealth, _stats._shield, _stats._resistance);

    }

    private void Start()
    {
        baseLayer = animator.GetLayerIndex("Movement");
        hitLayer = animator.GetLayerIndex("Hit");

        animator.SetLayerWeight(baseLayer, 1.0f);
        animator.SetLayerWeight(hitLayer, 1.0f);
    }

    public void TakeDamage(DamageInfo info)
    {
        animator.SetTrigger("Hit");

        bool died = _healthSystem.TakeDamage(info.Amount);

        Debug.Log($"{name} a pris {info.Amount} dégâts de {info.Attacker?.name}");

        // TODO : ici tu peux mettre :
        // - animation de hit
        // - écran rouge / shake

        if (died)
        {
            HandleDeath(info);
        }
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(new DamageInfo { Amount = amount, Attacker = null, Source = null });
    }

    private void HandleDeath(DamageInfo info)
    {
        Debug.Log($"{name} est mort.");
        animator.SetTrigger("Death");
    }

    public void Kill()
    {
        DropOrbs();
        Destroy(gameObject);
    }

    private void DropOrbs()
    {
        if (orbPrefab == null)
        {
            Debug.LogWarning($"EnemyHealth sur {name} : orbPrefab n'est pas assigné.");
            return;
        }

        int count = Random.Range(minOrbs, maxOrbs + 1);

        var usedPositions = new List<Vector3>();
        float minDistSqr = minDistanceBetweenOrbs * minDistanceBetweenOrbs;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = transform.position;
            const int maxAttempts = 10;
            int attempts = 0;
            bool valid = false;

            while (!valid && attempts < maxAttempts)
            {
                attempts++;

                // point aléatoire dans un disque autour de l'ennemi
                Vector2 circle = Random.insideUnitCircle * dropRadius;
                spawnPos = new Vector3(
                    transform.position.x + circle.x,
                    transform.position.y + dropHeightOffset,
                    transform.position.z + circle.y
                );

                valid = true;
                for (int j = 0; j < usedPositions.Count; j++)
                {
                    if ((usedPositions[j] - spawnPos).sqrMagnitude < minDistSqr)
                    {
                        valid = false;
                        break;
                    }
                }
            }

            usedPositions.Add(spawnPos);
            Instantiate(orbPrefab, spawnPos, Quaternion.identity);
        }
    }


    private void Update()
    {
        health = _healthSystem._health;
    }
}
