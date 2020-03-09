using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                    scenes.Add(comp.gameObject.scene);
                }
            });

            if (ownedWorld.Owner != null)
            {
                ownedWorld.Owner.OnBeginEntitiesInjectionFromGameObjectConversion(scenes);
                ownedWorld.Owner.OnEndEntitiesInjectionFromGameObjectConversion();
            }
            else
            {
                DebugService.LogWarning($"The owned world '{DstEntityManager.World.Name}' doesn't appear to have an owner.");
            }
        }
    }
}

[UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
public class OwnedWorldInjectionPostConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        if (DstEntityManager.World is IOwnedWorld ownedWorld)
        {
            if (ownedWorld.Owner != null)
            {
                ownedWorld.Owner.OnEndEntitiesInjectionFromGameObjectConversion();
            }
            else
            {
                DebugService.LogWarning($"The owned world '{DstEntityManager.World.Name}' doesn't appear to have an owner.");
            }
        }
    }
}