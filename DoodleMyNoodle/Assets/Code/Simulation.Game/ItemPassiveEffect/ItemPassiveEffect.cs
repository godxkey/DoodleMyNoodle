using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public abstract class ItemPassiveEffect
{
    public static LogChannel LogChannel = Log.CreateChannel("Item Passive Effect", activeByDefault: true);

    public struct ItemContext
    {
        public Entity InstigatorPawn;
        public Entity ItemEntity;

        public ItemContext(Entity instigatorPawn, Entity entity)
        {
            InstigatorPawn = instigatorPawn;
            ItemEntity = entity;
        }
    }

    public abstract void Equip(ISimWorldReadWriteAccessor accessor, ItemContext context);
    public abstract void Unequip(ISimWorldReadWriteAccessor accessor, ItemContext context);
    public virtual void Use(ISimWorldReadWriteAccessor accessor, ItemContext context) { }
    public virtual void OnPawnIntStatChanged(ISimWorldReadWriteAccessor accessor, ItemContext context, IStatInt Stat) { }
    public virtual void OnPawnFixStatChanged(ISimWorldReadWriteAccessor accessor, ItemContext context, IStatFix Stat) { }

    // Other events like new turn, etc.

    [System.Diagnostics.Conditional("UNITY_X_LOG_INFO")]
    protected void LogItemPassiveEffectInfo(ItemContext context, string message)
    {
        Log.Info(LogChannel, $"Item Passive Effect {message} - {GetType().Name} - context(item: {context.ItemEntity}, instigator: {context.InstigatorPawn})");
    }
}