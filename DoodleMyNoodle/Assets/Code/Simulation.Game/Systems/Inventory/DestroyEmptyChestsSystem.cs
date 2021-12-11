using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using static fixMath;
using static Unity.Mathematics.math;

public class DestroyEmptyChestsSystem : SimSystemBase
{
    EndSimulationEntityCommandBufferSystem _ecb;

    protected override void OnCreate()
    {
        base.OnCreate();
        _ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecb.CreateCommandBuffer();
        Entities
            .WithNone<Controllable>()
            .WithNone<StartingInventoryItem>()
            .WithNone<DynamicChestFormulaRef>()
            .ForEach((Entity chest, DynamicBuffer<InventoryItemReference> inventory) =>
            {
                if (inventory.Length == 0)
                {
                    ecb.DestroyEntity(chest);
                }
            }).Run();
    }
}