using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    [Header("Références joueur")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Stats playerStats;

    public enum StatType
    {
        Shield,
        Resistance,
        Speed,
        AttackDamage
    }

    [System.Serializable]
    public struct ShopBoost
    {
        public StatType stat;    // quelle stat on augmente
        public float amount;     // de combien
        public int price;        // prix en orbes
    }

    [Header("Items du shop")]
    [SerializeField] private ShopBoost[] boosts;

    public void Buy(int index)
    {
        if (index < 0 || index >= boosts.Length) return;

        var boost = boosts[index];

        playerInventory.HasEnoughOrbs(boost.price);

        playerInventory.Buy(boost.price);

        ApplyBoost(boost);

        Debug.Log("a acheté un boost");
    }

    private void ApplyBoost(ShopBoost item)
    {
        switch (item.stat)
        {
            case StatType.Shield:
                playerStats._shield += item.amount;
                break;
            case StatType.Resistance:
                playerStats._resistance += item.amount;
                break;
            case StatType.Speed:
                playerStats._speed += item.amount;
                break;
            case StatType.AttackDamage:
                playerStats._attackDamage += item.amount;
                break;
        }
    }
}
