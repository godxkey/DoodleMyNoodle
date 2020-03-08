using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

// These systems take care of notifying any 'owned worlds' that converted entities are being injected into it

[UpdateInGroup(typeof(GameObjectBeforeConversionGroup))]
public class OwnedWorldInjectionPreConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        if (DstEntityManager.World is IOwnedWorld ownedWorld)
        {
            //Entities.ForEach((Entity entity) =>
            //{
            //    if (EntityManager.HasComponent<InjectedInOwnedWorldFlag>(entity))
            //    {
            //        var comp = EntityManager.GetComponentObject<InjectedInOwnedWorldFlag>(entity);
            //        DebugService.Log($"{entity} coming from {comp.gameObject.scene.name}");
            //    }
            //    NativeArray<ComponentType> components = EntityManager.GetComponentTypes(entity, Unity.Collections.Allocator.Temp);

            //    StringBuilder stringBuilder = new StringBuilder();

            //    foreach (ComponentType item in components)
            //    {
            //        stringBuilder.Append($" {item.GetManagedType().Name} ");
            //    }
            //    DebugService.Log($"Entity {entity} has components:" + stringBuilder.ToString());

            //    components.Dispose();
            //});
            if (ownedWorld.Owner != null)
            {
                ownedWorld.Owner.OnBeginEntitiesInjectionFromGameObjectConversion();
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
            //Entities.ForEach((Entity entity) =>
            //{
            //    NativeArray<ComponentType> components = EntityManager.GetComponentTypes(entity, Unity.Collections.Allocator.Temp);

            //    StringBuilder stringBuilder = new StringBuilder();

            //    foreach (ComponentType item in components)
            //    {
            //        stringBuilder.Append($" {item.GetManagedType().Name} ");
            //    }
            //    DebugService.Log($"Entity {entity} has components:" + stringBuilder.ToString());

            //    components.Dispose();
            //});
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