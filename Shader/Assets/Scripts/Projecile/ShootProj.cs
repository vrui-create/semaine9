using Unity.VisualScripting;
using UnityEngine;

public class ShootProj : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Transform parent;
    [SerializeField] private Transform startPos;

    [Header("Prefab")]
    [SerializeField] private GameObject projObject;
    [SerializeField] private GameObject endAnimation;

    [Header("info")]
    [SerializeField] private int damage;
    [SerializeField] private string tagToHit;
    [SerializeField] private bool destroyOnHit;
    [SerializeField] private Stats ownerStats;

    private DamageDealer damageDealer;
    private Projectile proj;

    private void Awake()
    {
        ownerStats = GetComponent<Stats>();
    }

    public void Shoot()
    {
        GameObject projectile = Instantiate(projObject, startPos.position, parent.rotation);

        damageDealer = projectile.GetComponent<DamageDealer>();
        proj = projectile.GetComponent<Projectile>();

        damageDealer.Set(destroyOnHit, damage, tagToHit, true, ownerStats, endAnimation);

        
    }
}
