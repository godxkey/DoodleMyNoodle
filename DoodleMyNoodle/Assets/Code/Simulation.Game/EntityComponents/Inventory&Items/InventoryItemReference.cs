using System;
using Unity.Entities;

public struct InventoryItemReference : IBufferElementData
{
    public Entity ItemEntity;
    public int Stacks;
}

// used for item bundle added dynamicly while in game
public struct InventoryItemPrefabReference : IBufferElementData
{
    public Entity ItemEntityPrefab;
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
        where T : Action
    {
        return FindFirstItemWithGameAction(accessor, pawn, ActionBank.GetActionId<T>(), out _);
    }
    public static Entity FindFirstItemWithGameAction<T>(ISimWorldReadAccessor accessor, Entity pawn, out int itemIndex)
        where T : Action
    {
        return FindFirstItemWithGameAction(accessor, pawn, ActionBank.GetActionId<T>(), out itemIndex);
    }

    public static Entity FindFirstItemWithGameAction(ISimWorldReadAccessor accessor, Entity pawn, ActionId gameActionId)
    {
        return FindFirstItemWithGameAction(accessor, pawn, gameActionId, out _);
    }

    public static Entity FindFirstItemWithGameAction(ISimWorldReadAccessor accessor, Entity pawn, ActionId gameActionId, out int itemIndex)
    {
        if (accessor.TryGetBufferReadOnly(pawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (accessor.TryGetComponent(inventory[i].ItemEntity, out ActionId candidateGameActionId))
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
}