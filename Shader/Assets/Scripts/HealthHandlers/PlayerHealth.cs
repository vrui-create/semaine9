using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private HealthSystem _healthSystem;
    private Stats _stats;
    public float health;

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



    public void TakeDamage(DamageInfo info)
    {
        bool died = _healthSystem.TakeDamage(info.Amount);

        Debug.Log($"{name} a pris {info.Amount} dégâts de {info.Attacker?.name}");




        // TODO : ici tu peux mettre :
        // - animation de hit
        // - écran rouge / shake
        // - mise à jour d'UI

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
        // TODO : animation de mort, désactiver le mouvement, etc.
    }

    private void Update()
    {
        health = _healthSystem._health;
    }
}
