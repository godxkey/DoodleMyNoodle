using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineX.InspectorDisplay;

[DisallowMultipleComponent]
public class PeriodicActionAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [Suffix("seconds")]
    public float ActEvery = 1;

    public GameActionAuth Action = null;

    [Tooltip("If set to true, the attack progress will increase (up to 1) even if the actor is not in position for attack. When the pawn will be in position for attack, it will be able to fire earlier.")]
    public bool PrepareInAdvance = false;

    [Tooltip("Set to -1 for no limit")]
    public int Limit = -1;


    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<PeriodicActionRate>(entity, (fix)(1 / Mathf.Max(ActEvery, 0.0001f)));
        dstManager.AddComponentData<PeriodicActionProgress>(entity, (fix)(PrepareInAdvance ? 1 : 0));
        dstManager.AddComponentData<PeriodicActionEnabled>(entity, default);
        dstManager.AddComponentData<ProgressPeriodicActionInAdvance>(entity, PrepareInAdvance);
        dstManager.AddComponentData<RemainingPeriodicActionCount>(entity, Limit);
        dstManager.AddComponentData<PeriodicAction>(entity, Action != null ? conversionSystem.GetPrimaryEntity(Action.gameObject) : default);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (Action != null)
            referencedPrefabs.Add(Action.gameObject);
    }
}