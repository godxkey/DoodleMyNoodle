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
    // SIMULATION

    // Game Action
    public GameActionAuth ActionPrefab;

    public int ApCost = 1;

    public bool IsStackable = true;

    public enum CooldownMode
    {
        NoCooldown,
        Seconds,
    }

    public CooldownMode CooldownType = CooldownMode.NoCooldown;
    public float CooldownDuration = 1;

    public bool HideInInventory = false;

    public bool HasCooldown => CooldownType != CooldownMode.NoCooldown;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<ItemAction>(entity, ActionPrefab != null ? conversionSystem.GetPrimaryEntity(ActionPrefab.gameObject) : default);

        if (CooldownType == CooldownMode.Seconds)
        {
            dstManager.AddComponentData(entity, new ItemTimeCooldownData() { Value = (fix)CooldownDuration });
        }

        dstManager.AddComponentData(entity, new ItemSettingAPCost() { Value = ApCost });
        dstManager.AddComponentData(entity, new StackableFlag() { Value = IsStackable });
        dstManager.AddComponent<FirstInstigator>(entity);
        dstManager.AddComponent<ItemTag>(entity);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (ActionPrefab != null)
            referencedPrefabs.Add(ActionPrefab.gameObject);
    }

    // PRESENTATION

    // Description
    public Sprite Icon;
    public Color IconTint = Color.white;
    public string Name;
    public string EffectDescription;
}