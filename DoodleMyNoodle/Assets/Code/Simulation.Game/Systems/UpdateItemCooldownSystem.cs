using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(PreAISystemGroup))]
public class UpdateItemCooldownSystem : SimSystemBase
{
    EntityQuery _entitiesWithCooldown;

    protected override void OnCreate()
    {
        base.OnCreate();

        var queryDescription = new EntityQueryDesc()
        {
            Any = new ComponentType[] { typeof(ItemCooldownTimeCounter), typeof(ItemCooldownTurnCounter) }
        };
        _entitiesWithCooldown = GetEntityQuery(queryDescription);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var deltaTime = Time.DeltaTime;

        if (HasSingleton<NoCooldownTag>())
        {
            EntityManager.RemoveComponent(_entitiesWithCooldown, new ComponentTypes(typeof(ItemCooldownTimeCounter), typeof(ItemCooldownTurnCounter)));
        }

        Entities
            .ForEach((Entity item, ref ItemCooldownTimeCounter cooldownCounter) =>
            {
                cooldownCounter.Value -= deltaTime;
                if (cooldownCounter.Value <= 0)
                {
                    ecb.RemoveComponent<ItemCooldownTimeCounter>(item);
                }
            }).Run();

        if (HasSingleton<NewTurnEventData>())
        {
            var inventoryFromEntity = GetBufferFromEntity<InventoryItemReference>(isReadOnly: true);

            Team currentTeam = CommonReads.GetTurnTeam(Accessor);
            Entities
               .ForEach((Entity pawnController, ref ControlledEntity pawn, in Team team) =>
               {
                   if (team == currentTeam && inventoryFromEntity.HasComponent(pawn))
                   {
                       foreach (InventoryItemReference item in inventoryFromEntity[pawn])
                       {
                           if (HasComponent<ItemCooldownTurnCounter>(item.ItemEntity))
                           {
                               var itemTurnCounter = GetComponent<ItemCooldownTurnCounter>(item.ItemEntity);
                               itemTurnCounter.Value -= 1;
                               if (itemTurnCounter.Value <= 0)
                               {
                                   ecb.RemoveComponent<ItemCooldownTurnCounter>(item.ItemEntity);
                               }
                               else
                               {
                                   SetComponent(item.ItemEntity, new ItemCooldownTurnCounter() { Value = itemTurnCounter.Value });
                               }
                           }
                       }
                   }
               }).Run();

        }
        ecb.Playback(EntityManager);
    }
}
