using Unity.Entities;
using static Unity.Mathematics.math;
using static fixMath;
using System.Collections.Generic;

public interface IStatInt
{
    int Value { get; set; }
}

public interface IStatFix
{
    fix Value { get; set; }
}

public struct MinimumInt<T> : IComponentData
    where T : IComponentData, IStatInt
{
    public int Value;

    public static implicit operator int(MinimumInt<T> val) => val.Value;
    public static implicit operator MinimumInt<T>(int val) => new MinimumInt<T>() { Value = val };
}

public struct MaximumInt<T> : IComponentData
    where T : IComponentData, IStatInt
{
    public int Value;

    public static implicit operator int(MaximumInt<T> val) => val.Value;
    public static implicit operator MaximumInt<T>(int val) => new MaximumInt<T>() { Value = val };
}

public struct MinimumFix<T> : IComponentData
    where T : IComponentData, IStatFix
{
    public fix Value;

    public static implicit operator fix(MinimumFix<T> val) => val.Value;
    public static implicit operator MinimumFix<T>(fix val) => new MinimumFix<T>() { Value = val };
}

public struct MaximumFix<T> : IComponentData
    where T : IComponentData, IStatFix
{
    public fix Value;

    public static implicit operator fix(MaximumFix<T> val) => val.Value;
    public static implicit operator MaximumFix<T>(fix val) => new MaximumFix<T>() { Value = val };
}

internal static partial class CommonWrites
{
    public static void ModifyStatInt<T>(ISimWorldReadWriteAccessor accessor, Entity entity, int value)
        where T : struct, IComponentData, IStatInt
    {
        int currentValue = accessor.GetComponent<T>(entity).Value;
        int newValue = currentValue + value;

        SetStatInt(accessor, entity, new T { Value = newValue });
    }

    public static void ModifyStatFix<T>(ISimWorldReadWriteAccessor accessor, Entity entity, fix value)
        where T : struct, IComponentData, IStatFix
    {
        fix currentValue = accessor.GetComponent<T>(entity).Value;
        fix newValue = currentValue + value;

        SetStatFix(accessor, entity, new T { Value = newValue });
    }

    public static void SetStatInt<T>(ISimWorldReadWriteAccessor accessor, Entity entity, T compData)
        where T : struct, IComponentData, IStatInt
    {
        if (accessor.TryGetComponent(entity, out MinimumInt<T> minimum))
        {
            compData.Value = max(minimum.Value, compData.Value);
        }

        if (accessor.TryGetComponent(entity, out MaximumInt<T> maximum))
        {
            compData.Value = min(maximum.Value, compData.Value);
        }

        accessor.SetComponent(entity, compData);

        OnIntStatChanged(accessor, entity, compData);
    }

    public static void SetStatFix<T>(ISimWorldReadWriteAccessor accessor, Entity entity, T compData)
        where T : struct, IComponentData, IStatFix
    {
        if (accessor.TryGetComponent(entity, out MinimumFix<T> minimum))
        {
            compData.Value = max(minimum.Value, compData.Value);
        }

        if (accessor.TryGetComponent(entity, out MaximumFix<T> maximum))
        {
            compData.Value = min(maximum.Value, compData.Value);
        }

        accessor.SetComponent(entity, compData);

        OnFixStatChanged(accessor, entity, compData);
    }

    public static void AddStatFix<T>(ISimWorldReadWriteAccessor accessor, Entity entity, fix value)
    where T : struct, IComponentData, IStatFix
    {
        if (!accessor.HasComponent<T>(entity))
        {
            accessor.AddComponent(entity, new T { Value = value });
        }
    }

    public static void AddStatInt<T>(ISimWorldReadWriteAccessor accessor, Entity entity, int value)
    where T : struct, IComponentData, IStatInt
    {
        if (!accessor.HasComponent<T>(entity))
        {
            accessor.AddComponent(entity, new T { Value = value });
        }
    }

    public static void OnFixStatChanged<T>(ISimWorldReadWriteAccessor accessor, Entity entity, T ChangedStat)
    where T : struct, IComponentData, IStatFix
    {
        // if we added a stat to a pawn, notify its items of the change if he has any
        if (accessor.TryGetComponent(entity, out Controllable pawn))
        {
            if (accessor.TryGetBufferReadOnly(entity, out DynamicBuffer<InventoryItemReference> inventory))
            {
                foreach (InventoryItemReference ItemReference in inventory)
                {
                    if (accessor.TryGetBufferReadOnly(ItemReference.ItemEntity, out DynamicBuffer<ItemPassiveEffectId> itemPassiveEffectIds))
                    {
                        ItemPassiveEffect.ItemContext itemContext = new ItemPassiveEffect.ItemContext()
                        {
                            InstigatorPawn = entity,
                            ItemEntity = ItemReference.ItemEntity
                        };

                        foreach (ItemPassiveEffectId itemPassiveEffectId in itemPassiveEffectIds)
                        {
                            ItemPassiveEffect itemPassiveEffect = ItemPassiveEffectBank.GetItemPassiveEffect(itemPassiveEffectId);
                            if (itemPassiveEffect != null)
                            {
                                itemPassiveEffect.OnPawnFixStatChanged(accessor, itemContext, ChangedStat);
                            }
                        }
                    }
                }
            }
        }
    }

    public static void OnIntStatChanged<T>(ISimWorldReadWriteAccessor accessor, Entity entity, T ChangedStat)
    where T : struct, IComponentData, IStatInt
    {
        // if we added a stat to a pawn, notify its items of the change if he has any
        if (accessor.TryGetComponent(entity, out Controllable pawn))
        {
            if (accessor.TryGetBufferReadOnly(entity, out DynamicBuffer<InventoryItemReference> inventory))
            {
                foreach (InventoryItemReference ItemReference in inventory)
                {
                    if (accessor.TryGetBufferReadOnly(ItemReference.ItemEntity, out DynamicBuffer<ItemPassiveEffectId> itemPassiveEffectIds))
                    {
                        ItemPassiveEffect.ItemContext itemContext = new ItemPassiveEffect.ItemContext()
                        {
                            InstigatorPawn = entity,
                            ItemEntity = ItemReference.ItemEntity
                        };

                        foreach (ItemPassiveEffectId itemPassiveEffectId in itemPassiveEffectIds)
                        {
                            ItemPassiveEffect itemPassiveEffect = ItemPassiveEffectBank.GetItemPassiveEffect(itemPassiveEffectId);
                            if (itemPassiveEffect != null)
                            {
                                itemPassiveEffect.OnPawnIntStatChanged(accessor, itemContext, ChangedStat);
                            }
                        }
                    }
                }
            }
        }
    }
}


