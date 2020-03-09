using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class ConvertToEntityMultiWorld : ConvertToEntity
{
    public abstract GameWorldType WorldToConvertTo { get; }

    void Awake()
    {
        if (World.DefaultGameObjectInjectionWorld != null)
        {
            World dstWorld = GetAssociatedWorld();
            if(dstWorld != null)
            {
                if(dstWorld is IOwnedWorld)
                {
                    gameObject.AddComponent<InjectedInOwnedWorldFlag>();
                }

                var convertSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>();
                convertSystem.AddToBeConverted(dstWorld, this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning($"{nameof(ConvertToEntity)} failed because there is no {nameof(World.DefaultGameObjectInjectionWorld)}", this);
        }
    }

    World GetAssociatedWorld()
    {
        switch (WorldToConvertTo)
        {
            case GameWorldType.Simulation:
                if (Game.Ready)
                    return World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationWorldSystem>().SimulationWorld;
                else
                    return null;
            
            case GameWorldType.Presentation:
                return World.DefaultGameObjectInjectionWorld;

            default:
                return null;
        }
    }
}
