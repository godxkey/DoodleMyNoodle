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
    public GameObject GameActionEntityPrefab;

    public int ApCost = 1;

    public bool IsStackable = true;

    public enum CooldownMode
    {
        NoCooldown,
        Seconds,
        Turns
    }

    public CooldownMode CooldownType = CooldownMode.NoCooldown;
    public fix CooldownDuration = 1;

    public bool HideInInventory = false;

    [FormerlySerializedAs("CanBeUsedAtAnytime")]
    public bool UsableInOthersTurn = false;

    public bool HasCooldown => CooldownType != CooldownMode.NoCooldown;

    public bool IsAutoUse = false;
    public fix PassiveUsageTimeInterval = 1;
    public Action.UseParameters DefaultParamaters = new Action.UseParameters();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemAction() { ActionPrefab = conversionSystem.GetPrimaryEntity(GameActionEntityPrefab) });

        if (CooldownType == CooldownMode.Seconds)
        {
            dstManager.AddComponentData(entity, new ItemTimeCooldownData() { Value = CooldownDuration });
        }
        else if (CooldownType == CooldownMode.Turns)
        {
            dstManager.AddComponentData(entity, new ItemTurnCooldownData() { Value = fixMath.roundToInt(CooldownDuration) });
        }

        dstManager.AddComponentData(entity, new ActionSettingAPCost() { Value = ApCost });
        dstManager.AddComponentData(entity, new StackableFlag() { Value = IsStackable });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(GameActionEntityPrefab);
    }

    // PRESENTATION

    // Description
    public Sprite Icon;
    public Color IconTint = Color.white; // 0008FF
    public string Name;
    public string EffectDescription;
}