using System;
using Unity.Entities;

public struct InventoryItemReference : IBufferElementData
{
    public Entity ItemEntity;
    public int Stacks;
}

// Used for default base inventory when creating it
public struct StartingInventoryItem : IBufferElementData
{
    public SimAssetId ItemAssetId;
    public int StacksMin;
    public int StacksMax;
}

public struct StackableFlag : IComponentData
{
    public bool Value;

    public static implicit operator bool(StackableFlag val) => val.Value;
    public static implicit operator StackableFlag(bool val) => new StackableFlag() { Value = val };
}

public partial class CommonReads
{
    public static Entity FindFirstItemWithGameAction<T>(ISimWorldReadAccessor accessor, Entity pawn)
        where T : GameAction
    {
        return FindFirstItemWithGameAction(accessor, pawn, GameActionBank.GetActionId<T>(), out _);
    }
    public static Entity FindFirstItemWithGameAction<T>(ISimWorldReadAccessor accessor, Entity pawn, out int itemIndex)
        where T : GameAction
    {
        return FindFirstItemWithGameAction(accessor, pawn, GameActionBank.GetActionId<T>(), out itemIndex);
    }

    public static Entity FindFirstItemWithGameAction(ISimWorldReadAccessor accessor, Entity pawn, GameActionId gameActionId)
    {
        return FindFirstItemWithGameAction(accessor, pawn, gameActionId, out _);
    }

    public static Entity FindFirstItemWithGameAction(ISimWorldReadAccessor accessor, Entity pawn, GameActionId gameActionId, out int itemIndex)
    {
        if (accessor.TryGetBufferReadOnly(pawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (accessor.TryGetComponent(inventory[i].ItemEntity, out GameActionId candidateGameActionId))
                {
                    if (candidateGameActionId.Equals(gameActionId))
                    {
                        itemIndex = i;
                        return inventory[i].ItemEntity;
                    }
                }
            }
        }

        itemIndex = -1;
        return Entity.Null;
    }

    public static int FindItemIndex(ISimWorldReadAccessor accessor, Entity pawn, Entity itemEntity)
    {
        if (accessor.TryGetBufferReadOnly(pawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].ItemEntity == itemEntity)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public static Entity FindItemByIndex(ISimWorldReadAccessor accessor, Entity pawn, int itemIndex)
    {
        if (accessor.TryGetBufferReadOnly(pawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            if (itemIndex >= 0 && itemIndex <= inventory.Length - 1)
            {
                return inventory[itemIndex].ItemEntity;
            }
        }

        return Entity.Null;
    }
}