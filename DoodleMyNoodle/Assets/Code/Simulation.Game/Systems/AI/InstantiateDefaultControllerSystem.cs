using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

/// <summary>
/// This system instantiates controllable entities' DefaultControllerPrefab if they have the 'InstantiateAndUseDefaultController' tag
/// </summary>
public class InstantiateDefaultControllerSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<InstantiateAndUseDefaultControllerTag>()
            .ForEach((Entity pawnEntity, ref DefaultControllerPrefab defaultControllerPrefab) =>
            {
                Entity newController = PostUpdateCommands.Instantiate(defaultControllerPrefab.Value);

                PostUpdateCommands.SetComponent(newController, new ControlledEntity() { Value = pawnEntity });

                PostUpdateCommands.RemoveComponent<InstantiateAndUseDefaultControllerTag>(pawnEntity);
            });
    }
}