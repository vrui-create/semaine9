using UnityEngine;

public class HealthSystem
{
    public float _health;
    protected float _maxHealth;
    protected float _shield;
    protected float _resistance;

    public float Health => _health;
    public float MaxHealth => _maxHealth;
    public float Shield => _shield;
    public float Resistance => _resistance;

    public HealthSystem(float maxHealth, float shield, float resistance)
    {
        _maxHealth = maxHealth;
        _health = maxHealth;
        _shield = shield;
        _resistance = resistance;
    }

    public void SetMaxHealth(float maxHealth) { _maxHealth = maxHealth; }
    public void SetShield(float shield) { _shield = shield; }
    public void SetResistance(float resistance) { _resistance = resistance; }


    public bool TakeDamage(float damage)
    {
        if (damage <= 0) return false;

        float remaining = damage;

        // 1) Le shield absorbe en premier
        if (_shield > 0)
        {
            float absorbed = Mathf.Min(_shield, remaining);
            _shield -= absorbed;
            remaining -= absorbed;
        }

        if (remaining <= 0)
            return false;

        // 2) Résistance : 0–100% de réduction
        float reductionFactor = Mathf.Clamp01(_resistance / 100f);
        float finalDamage = remaining * (1f - reductionFactor);

        _health -= finalDamage;

        if (_health <= 0)
        {
            _health = 0;
            return true;
        }

        return false;
    }

    public void HealPercent(float healPercentage)
    {
        _health += _maxHealth * (healPercentage / 100f);
        if (_health > _maxHealth) _health = _maxHealth;
    }

    public float GetPercentageHealth()
    {
        return (_health / _maxHealth) * 100f;
    }
}
