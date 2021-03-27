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
                if (HasSingleton<NoCooldownTag>())
                {
                    PostUpdateCommands.RemoveComponent<ItemCooldownTimeCounter>(item);
                }
                else
                {
                    cooldownCounter.Value -= deltaTime;
                    if (cooldownCounter.Value <= 0)
                    {
                        PostUpdateCommands.RemoveComponent<ItemCooldownTimeCounter>(item);
                    }
                }
            });

        if (HasSingleton<NewTurnEventData>())
        {
            Team currentTeam = CommonReads.GetTurnTeam(Accessor);
            Entities
           .ForEach((Entity pawnController, ref ControlledEntity pawn) =>
           {
               if (EntityManager.TryGetComponentData(pawnController, out Team team) && team == currentTeam)
               {
                   if (EntityManager.Exists(pawn) && EntityManager.TryGetBuffer(pawn, out DynamicBuffer<InventoryItemReference> inventory))
                   {
                       foreach (InventoryItemReference item in inventory)
                       {
                           if (EntityManager.TryGetComponentData(item.ItemEntity, out ItemCooldownTurnCounter itemTurnCounter))
                           {
                               if (HasSingleton<NoCooldownTag>())
                               {
                                   PostUpdateCommands.RemoveComponent<ItemCooldownTurnCounter>(item.ItemEntity);
                               }
                               else
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
