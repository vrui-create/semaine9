using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private DamageType damageType = DamageType.Physical;
    [SerializeField] private bool isProj;

    [Header("Cible")]
    [SerializeField] private string targetTag = "Player";  // tag de ce que tu veux toucher
    [SerializeField] private LayerMask hittableLayers = ~0;

    [Header("Références")]
    [SerializeField] private GameObject owner;  // généralement le joueur ou l'ennemi
    [SerializeField] private Stats attackerStats;
    
    private GameObject endAnimation;

    [SerializeField] private bool destroyOnHit = false;


    public void Set(bool _desTroyOnHit, float _baseDamage, string TargetTag, bool _isProj, 
                    Stats _attackerStat = null, GameObject _endAnimation = null)
    {
        destroyOnHit = _desTroyOnHit;
        baseDamage = _baseDamage;
        targetTag = TargetTag;
        attackerStats = _attackerStat;
        isProj = _isProj;
        endAnimation = _endAnimation;
    }

    private void Awake()
    {
        if (owner == null && !isProj)
            owner = transform.root.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProj && owner != null && other.transform.root == owner.transform.root)
            return;

        if (((1 << other.gameObject.layer) & hittableLayers) == 0)
            return;

        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
            return;

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null)
            return;

        // 4) calcule les dégâts finaux
        float statsDamage;

        if (attackerStats != null)  statsDamage = attackerStats._attackDamage;
        else statsDamage = 0f;

        float finalDamage = baseDamage + statsDamage;

        var info = new DamageInfo
        {
            Amount = finalDamage,
            Type = damageType,
            Attacker = owner,
            Source = gameObject
        };

        damageable.TakeDamage(info);

        if (destroyOnHit) EndAnimation();
    }

    public void EndAnimation()
    {
        if (endAnimation != null) Instantiate(endAnimation, transform.position, Quaternion.identity);
        Destroy(this);
    }
}
