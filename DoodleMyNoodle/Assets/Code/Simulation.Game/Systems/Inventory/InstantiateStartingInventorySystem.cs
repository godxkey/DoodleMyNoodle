using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class InstantiateStartingInventorySystem : SimSystemBase
{
    private GlobalItemBankSystem _itemBankSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _itemBankSystem = World.GetOrCreateSystem<GlobalItemBankSystem>();

        RequireSingletonForUpdate<GlobalItemBankTag>();
    }

    protected override void OnUpdate()
    {
        Entities
            .WithAll<InventoryItemReference>()
            .ForEach((Entity entity, DynamicBuffer<StartingInventoryItem> startingInventoryBuffer) =>
            {
                NativeArray<StartingInventoryItem> startingInventory = startingInventoryBuffer.ToNativeArray(Allocator.Temp);
                NativeArray<(Entity item, int stack)> itemTransfers = new NativeArray<(Entity item, int stack)>(startingInventoryBuffer.Length, Allocator.Temp);

                var random = World.Random();

                for (int i = 0; i < startingInventory.Length; i++)
                {
                    var itemInstance = _itemBankSystem.GetItemInstance(startingInventory[i].ItemAssetId);
                    itemTransfers[i] = (itemInstance, random.NextInt(startingInventory[i].StacksMin, startingInventory[i].StacksMax));
                }

                ItemTransationBatch itemTransationBatch = new ItemTransationBatch()
                {
                    Source = null,
                    Destination = entity,
                    ItemsAndStacks = itemTransfers,
                    OutResults = default
                };

                CommonWrites.ExecuteItemTransaction(Accessor, itemTransationBatch);

                // Passive stuff
                foreach (var item in GetBuffer<InventoryItemReference>(entity))
                {
                    ItemPassiveEffect.ItemContext itemContext = new ItemPassiveEffect.ItemContext()
                    {
                        InstigatorPawn = entity,
                        ItemEntity = item.ItemEntity
                    };

                    if (EntityManager.TryGetBuffer(item.ItemEntity, out DynamicBuffer<ItemPassiveEffectId> itemPassiveEffectIds))
                    {
                        foreach (ItemPassiveEffectId itemPassiveEffectId in itemPassiveEffectIds)
                        {
                            ItemPassiveEffect itemPassiveEffect = ItemPassiveEffectBank.GetItemPassiveEffect(itemPassiveEffectId);
                            if (itemPassiveEffect != null)
                            {
                                itemPassiveEffect.Equip(Accessor, itemContext);
                            }
                        }
                    }
                }

                // Remove starting inventory buffer
                EntityManager.RemoveComponent<StartingInventoryItem>(entity);
            })
            .WithoutBurst()
            .WithStructuralChanges()
            .Run();
    }
}
