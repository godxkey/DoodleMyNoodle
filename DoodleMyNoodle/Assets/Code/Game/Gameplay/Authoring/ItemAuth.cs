using Unity.Entities;
using UnityEngine;
using System.Linq;
using System;
using CCC.InspectorDisplay;
using System.Collections.Generic;
using UnityEngineX.InspectorDisplay;
using UnityEngineX;
using System.Reflection;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class ItemAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public enum ItemTier
    {
        Tier1,
        Tier2,
        Tier3,
        Tier4,
        Tier5,
    }

    [Header("Simulation")]
    public List<SpellAuth> SpellPrefabs = new List<SpellAuth>();
    public bool AvailableInShop = true;
    public ItemTier Tier = ItemTier.Tier1;
    public bool HasCharges = true;
    [ShowIf(nameof(HasCharges))]
    public int ChargeCount = 10;

    [Header("Presentation")]
    public Sprite Icon;
    public Color IconTint = Color.white;
    public TextData Name;
    public TextData Description;
    public bool HideInInventory = false;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spellBuffer = dstManager.AddBuffer<ItemSpell>(entity);
        foreach (var spell in SpellPrefabs)
        {
            if (spell != null)
                spellBuffer.Add(conversionSystem.GetPrimaryEntity(spell));
        }
        dstManager.AddComponentData(entity, new StackableFlag() { Value = false });
        if (HasCharges)
        {
            dstManager.AddComponentData(entity, new ItemCharges() { Value = ChargeCount });
            dstManager.AddComponentData(entity, new ItemStartingCharges() { Value = ChargeCount });
        }
        dstManager.AddComponent<Owner>(entity);
        dstManager.AddComponent<ItemTag>(entity);
        dstManager.AddComponent<ItemCurrentSpellIndex>(entity);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var spell in SpellPrefabs)
        {
            if (spell != null)
                referencedPrefabs.Add(spell.gameObject);
        }
    }
}