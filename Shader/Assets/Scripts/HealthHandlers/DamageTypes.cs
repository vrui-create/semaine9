using UnityEngine;

public enum DamageType
{
    Physical,
    True // ignore la resistance, si tu veux
}

public struct DamageInfo
{
    public float Amount;
    public DamageType Type;
    public GameObject Attacker; // qui attaque (owner)
    public GameObject Source;   // d'ou vient le hit (hitbox, projectile…)
}

public interface IDamageable
{
    void TakeDamage(DamageInfo info);
}
