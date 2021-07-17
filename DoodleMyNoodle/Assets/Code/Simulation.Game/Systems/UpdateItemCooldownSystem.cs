using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(PreAISystemGroup))]
public class UpdateItemCooldownSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        bool noCooldownSingleton = HasSingleton<NoCooldownTag>();

        Entities
            .ForEach((Entity item, ref ItemCooldownTimeCounter cooldownCounter) =>
            {
                if (noCooldownSingleton)
                {
                    EntityManager.RemoveComponent<ItemCooldownTimeCounter>(item);
                }
                else
                {
                    cooldownCounter.Value -= deltaTime;
                    if (cooldownCounter.Value <= 0)
                    {
                        EntityManager.RemoveComponent<ItemCooldownTimeCounter>(item);
                    }
                }
            }).WithStructuralChanges().WithoutBurst().Run();

        if (HasSingleton<NewTurnEventData>())
        {
            Team currentTeam = CommonReads.GetTurnTeam(Accessor);
            Entities
               .ForEach((Entity pawnController, ref ControlledEntity pawn, in Team team) =>
               {
                   if (team == currentTeam && EntityManager.TryGetBuffer(pawn, out DynamicBuffer<InventoryItemReference> inventory))
                   {
                       foreach (InventoryItemReference item in inventory)
                       {
                           if (TryGetComponent(item.ItemEntity, out ItemCooldownTurnCounter itemTurnCounter))
                           {
                               if (noCooldownSingleton)
                               {
                                   EntityManager.RemoveComponent<ItemCooldownTurnCounter>(item.ItemEntity);
                               }
                               else
                               {
                                   itemTurnCounter.Value -= 1;
                                   if (itemTurnCounter.Value <= 0)
                                   {
                                       EntityManager.RemoveComponent<ItemCooldownTurnCounter>(item.ItemEntity);
                                   }
                                   else
                                   {
                                       SetComponent(item.ItemEntity, new ItemCooldownTurnCounter() { Value = itemTurnCounter.Value });
                                   }
                               }
                           }
                       }
                   }
               }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}
