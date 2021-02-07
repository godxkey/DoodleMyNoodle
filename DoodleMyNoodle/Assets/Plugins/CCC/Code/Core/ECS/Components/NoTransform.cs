using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class NoTransform : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        RemoveTransformComponents(entity, dstManager, conversionSystem);
    }

    public static void RemoveTransformComponents(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.RemoveComponent<Rotation>(entity);
        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<LocalToWorld>(entity);
        dstManager.RemoveComponent<LinkedEntityGroup>(entity);
        dstManager.AddComponent<RemoveTransformInConversionTag>(entity);
    }
}

public struct RemoveTransformInConversionTag : IComponentData
{

}

[UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
[ConverterVersion("fred", 1)]
class RemoveTransformConversionSystem : GameObjectConversionSystem
{
    private void Convert(Transform transform)
    {
        var entity = GetPrimaryEntity(transform);

        if(DstEntityManager.HasComponent<RemoveTransformInConversionTag>(entity))
        {
            DstEntityManager.RemoveComponent<RemoveTransformInConversionTag>(entity);

            if (DstEntityManager.HasComponent<LocalToWorld>(entity))
                DstEntityManager.RemoveComponent<LocalToWorld>(entity);
        }
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Transform transform) =>
        {
            Convert(transform);
        });

        //@TODO: Remove this again once we add support for inheritance in queries
        Entities.ForEach((RectTransform transform) =>
        {
            Convert(transform);
        });
    }
}