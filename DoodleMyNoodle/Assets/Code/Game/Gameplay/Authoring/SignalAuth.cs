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
    public List<SignalAuth> LogicTargets = new List<SignalAuth>();
    //public List<SignalAuth> PropagationTargets = new List<SignalAuth>();

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Signal>(entity);
        dstManager.AddComponent<PreviousSignal>(entity);

        if (Emission != ESignalEmissionType.None)
        {
            dstManager.AddComponent<SignalEmissionFlags>(entity);
            dstManager.AddComponentData<InteractableFlag>(entity, Emission == ESignalEmissionType.OnClick || Emission == ESignalEmissionType.ToggleOnClick);
            dstManager.AddComponentData<SignalEmissionType>(entity, Emission);

            var logicTargetsECS = dstManager.AddBuffer<SignalLogicTarget>(entity);
            foreach (var target in LogicTargets)
            {
                if (target != null)
                    logicTargetsECS.Add(conversionSystem.GetPrimaryEntity(target.gameObject));
            }
        }

        dstManager.AddComponentData<SignalStayOnForever>(entity, StayOnForever);
        //var targetsECS = dstManager.AddBuffer<SignalPropagationTarget>(entity);

        //foreach (var target in PropagationTargets)
        //{
        //    if (target != null)
        //        targetsECS.Add(conversionSystem.GetPrimaryEntity(target.gameObject));
        //}

    }

    void OnValidate()
    {
        for (int i = LogicTargets.Count - 1; i >= 0; i--)
        {
            if (LogicTargets[i] != null && !LogicTargets[i].gameObject.scene.IsValid())
            {
                LogicTargets.RemoveAt(i);
                Log.Error($"Cannot reference an asset. {nameof(LogicTargets)} needs to be a scene reference");
            }
        }
    }
}
