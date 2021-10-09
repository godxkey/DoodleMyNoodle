using CCC.InspectorDisplay;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
public class SignalAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool StayOnForever = false;
    public ESignalEmissionType Emission = ESignalEmissionType.None;
    [ShowIf(nameof(EmissionIsLogic))]
    public List<SignalAuth> LogicTargets = new List<SignalAuth>();
    public List<SignalAuth> PropagationTargets = new List<SignalAuth>();

    private bool EmissionIsLogic => Emission == ESignalEmissionType.OR || Emission == ESignalEmissionType.AND;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Signal>(entity);
        dstManager.AddComponent<PreviousSignal>(entity);
        dstManager.AddComponent<SignalEmissionFlags>(entity);
        dstManager.AddComponentData<SignalStayOnForever>(entity, StayOnForever);
        dstManager.AddComponentData<InteractableFlag>(entity, Emission == ESignalEmissionType.OnClick || Emission == ESignalEmissionType.ToggleOnClick);
        dstManager.AddComponentData<SignalEmissionType>(entity, Emission);
        var targetsECS = dstManager.AddBuffer<SignalPropagationTarget>(entity);

        foreach (var target in PropagationTargets)
        {
            if (target != null)
                targetsECS.Add(conversionSystem.GetPrimaryEntity(target.gameObject));
        }

        var logicTargetsECS = dstManager.AddBuffer<SignalLogicTarget>(entity);
        foreach (var target in LogicTargets)
        {
            if (target != null)
                logicTargetsECS.Add(conversionSystem.GetPrimaryEntity(target.gameObject));
        }
    }

    void OnValidate()
    {
        if (PropagationTargets != null)
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
}
