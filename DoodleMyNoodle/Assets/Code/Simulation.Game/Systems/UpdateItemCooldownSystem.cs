using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(PreAISystemGroup))]
public class UpdateItemCooldownSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities
            .ForEach((Entity item, ref ItemCooldownTimeCounter cooldownCounter) =>
            {
                cooldownCounter.Value -= deltaTime;
                if (cooldownCounter.Value <= 0)
                {
                    PostUpdateCommands.RemoveComponent<ItemCooldownTimeCounter>(item);
                }
            });

        if (HasSingleton<NewTurnEventData>())
        {
            Entities
           .ForEach((Entity pawnController, ref ControlledEntity controlledEntity) =>
           {
               if (EntityManager.TryGetComponentData(pawnController, out Team currentTeam))
               {
                   if (currentTeam.Value == CommonReads.GetTurnTeam(Accessor))
                   {
                       Entity pawn = controlledEntity.Value;
                       if (pawn != Entity.Null)
                       {
                           foreach (InventoryItemReference item in EntityManager.GetBufferReadOnly<InventoryItemReference>(pawn))
                           {
                               if (EntityManager.TryGetComponentData(item.ItemEntity, out ItemCooldownTurnCounter itemTurnCounter))
                               {
                                   itemTurnCounter.Value -= 1;
                                   if (itemTurnCounter.Value <= 0)
                                   {
                                       PostUpdateCommands.RemoveComponent<ItemCooldownTurnCounter>(item.ItemEntity);
                                   }
                                   else
                                   {
                                       EntityManager.SetComponentData(item.ItemEntity, new ItemCooldownTurnCounter() { Value = itemTurnCounter.Value });
                                   }
                               }
                           }
                       }
                   }
               }
           });
        }
    }
}
