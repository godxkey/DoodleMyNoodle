using System;
using Unity.Entities;

public struct InventoryItemReference : IBufferElementData
{
    public Entity ItemEntity;
}

// used for item bundle added dynamicly while in game
public struct InventoryItemPrefabReference : IBufferElementData
{
    public Entity ItemEntityPrefab;
}

// Used for default base inventory when creating it
public struct StartingInventoryItem : IBufferElementData
{
    public Entity ItemEntityPrefab;
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
                if (accessor.TryGetComponentData(inventory[i].ItemEntity, out GameActionId candidateGameActionId))
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