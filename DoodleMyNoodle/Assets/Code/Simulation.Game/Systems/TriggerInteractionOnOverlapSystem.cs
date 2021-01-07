using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class TriggerInteractionOnOverlapSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref TileActorOverlapBeginEventData overlapEvent) =>
        {
            OnEntityOverlap(overlapEvent.TileActorA, overlapEvent.TileActorB);
            OnEntityOverlap(overlapEvent.TileActorB, overlapEvent.TileActorA);
        });
    }

    private void OnEntityOverlap(Entity a, Entity b)
    {
        if (EntityManager.HasComponent<Controllable>(a) &&
            EntityManager.HasComponent<InteractOnOverlapTag>(b) &&
            EntityManager.HasComponent<Interactable>(b))
        {
            CommonWrites.Interact(Accessor, b, a);
        }
    }
}

internal partial class CommonWrites
{
    public static void Interact(ISimWorldReadWriteAccessor accessor, Entity interactableEntity, Entity Instigator)
    {
        if (accessor.HasComponent<RandomInventoryInfo>(interactableEntity))
        {
            OnChestToFillOppened(accessor, interactableEntity);
        }

        if (accessor.HasComponent<NoInteractTimer>(interactableEntity))
        {
            NoInteractTimer interactableTimer = accessor.GetComponentData<NoInteractTimer>(interactableEntity);
            fix endTime = accessor.Time.ElapsedTime + interactableTimer.Duration;
            accessor.SetComponentData(interactableEntity, new NoInteractTimer() { Duration = interactableTimer.Duration, EndTime = endTime, CanCountdown = true });
        }

        accessor.SetOrAddComponentData(interactableEntity, new Interacted() { Value = true, Instigator = Instigator });
    }

    private static void OnChestToFillOppened(ISimWorldReadWriteAccessor accessor, Entity interactableEntity)
    {
        NativeArray<ItemBankPrefabReference> itemBank;

        if (accessor.TryGetBufferReadOnly(interactableEntity, out DynamicBuffer<ItemBankPrefabReference> CustomItemBank))
        {
            // Custom Bank on Chest directly
            itemBank = CustomItemBank.ToNativeArray(Allocator.Temp);
        }
        else
        {
            // Using Global Item Bank
            Entity globalItemBankEntity = accessor.GetSingletonEntity<GlobalItemPrefabBankSingletonTag>();
            itemBank = accessor.GetBufferReadOnly<ItemBankPrefabReference>(globalItemBankEntity).ToNativeArray(Allocator.Temp);
        }

        NativeArray<Entity> itemInstances = new NativeArray<Entity>(itemBank.Length, Allocator.Temp);

        int amountToFill = accessor.GetComponentData<InventoryCapacity>(interactableEntity).Value;

        // Spawn items
        FixRandom randomNumbers = accessor.Random();
        for (int i = 0; i < amountToFill; i++)
        {
            itemInstances[i] = accessor.Instantiate(itemBank[randomNumbers.NextInt(0, itemBank.Length)].ItemEntityPrefab);
        }

        // Add item references into inventory
        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(interactableEntity);

        foreach (Entity itemInstance in itemInstances)
        {
            if (!CommonReads.IsInventoryFull(accessor, interactableEntity))
            {
                if (!TryIncrementStackableItemInInventory(accessor, interactableEntity, itemInstance, inventory))
                {
                    inventory.Add(new InventoryItemReference() { ItemEntity = itemInstance });
                }
                else
                {
                    if (accessor.TryGetComponentData(interactableEntity,out RandomInventoryInfo inventoryInfo))
                    {
                        int stackCount = 1;
                        int stackMax = randomNumbers.NextInt(1, inventoryInfo.MaxConsumableAmount);
                        while (stackCount < stackMax)
                        {
                            TryIncrementStackableItemInInventory(accessor, interactableEntity, itemInstance, inventory);
                            stackCount++;
                        }
                    }
                }
            }
        }

        itemInstances.Dispose();
        itemBank.Dispose();
    }
}