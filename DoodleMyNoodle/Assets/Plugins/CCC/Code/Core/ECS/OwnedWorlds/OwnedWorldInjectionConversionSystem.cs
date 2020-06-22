using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngineX;

// These systems take care of notifying any 'owned worlds' that converted entities are being injected into it

[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
public class OwnedWorldInjectionPreConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        if (DstEntityManager.World is IOwnedWorld ownedWorld)
        {
            List<Scene> scenes = new List<Scene>();

            Entities.ForEach((Entity entity) =>
            {
                if (EntityManager.HasComponent<InjectedInOwnedWorldFlag>(entity))
                {
                    var comp = EntityManager.GetComponentObject<InjectedInOwnedWorldFlag>(entity);
                    scenes.AddUnique(comp.gameObject.scene);
                }
            });

            if (ownedWorld.Owner != null)
            {
                ownedWorld.Owner.OnBeginEntitiesInjectionFromGameObjectConversion(scenes);

                var postInjectSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<OwnedWorldInjectionPostConversionSystem>();
                postInjectSystem.WorldOwnersToNotify.Add(ownedWorld.Owner);
            }
            else
            {
                Log.Warning($"The owned world '{DstEntityManager.World.Name}' doesn't appear to have an owner.");
            }
        }
    }
}

[UpdateAfter(typeof(ConvertToEntitySystem))]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class OwnedWorldInjectionPostConversionSystem : ComponentSystem
{
    public List<IWorldOwner> WorldOwnersToNotify = new List<IWorldOwner>();

    protected override void OnUpdate()
    {
        foreach (var worldOwner in WorldOwnersToNotify)
        {
            worldOwner.OnEndEntitiesInjectionFromGameObjectConversion();
        }
        WorldOwnersToNotify.Clear();
    }
}