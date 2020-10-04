using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Items/Item Visuel Info")]
public class ItemVisualInfo : ScriptableObject
{
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Mythic,
        Legendary
    }

    public SimAssetIdAuth ID;
    public Sprite Icon;
    public string Name;
    public ItemRarity Rarity;
    public string EffectDescription;
    
    public GameObject ItemPrefab => ID.gameObject;
}
