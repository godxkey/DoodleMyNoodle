using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SignalAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public ESignalEmissionType Emission = ESignalEmissionType.None;
    public List<SignalAuth> PropagationTargets;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Signal>(entity);
        dstManager.AddComponent<PreviousSignal>(entity);
        dstManager.AddComponentData<SignalEmissionType>(entity, Emission);
        dstManager.AddComponent<SignalEmissionFlags>(entity);
        var targetsECS = dstManager.AddBuffer<SignalPropagationTarget>(entity);

        foreach (var target in PropagationTargets)
        {
            if (target != null)
                targetsECS.Add(conversionSystem.GetPrimaryEntity(target.gameObject));
        }
    }

    void OnValidate()
    {
        for (int i = PropagationTargets.Count - 1; i >= 0; i--)
        {
            if (PropagationTargets[i] != null && !PropagationTargets[i].gameObject.scene.IsValid())
            {
                PropagationTargets.RemoveAt(i);
                Log.Error($"Cannot reference an asset. {nameof(PropagationTargets)} needs to be a scene reference");
            }
        }
    }
}