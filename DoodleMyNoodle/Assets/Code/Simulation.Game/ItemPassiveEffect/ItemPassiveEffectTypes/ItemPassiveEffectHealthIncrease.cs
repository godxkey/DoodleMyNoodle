using System;
using UnityEngine;
using UnityEngineX;

public class ItemPassiveEffectHealthIncrease : ItemPassiveEffect
{
    public override void Equip(ISimWorldReadWriteAccessor accessor, ItemContext context)
    {
        if (accessor.TryGetComponentData(context.ItemEntity, out ItemPassiveEffectHealthIncreaseData healthIncreaseData))
        {
            if (accessor.TryGetComponentData(context.InstigatorPawn, out Health hp))
            {
                fix valueToAdd = healthIncreaseData.Value;
                if (accessor.TryGetComponentData(context.InstigatorPawn, out HealthIncreaseMultiplier hpIncreaseBoost))
                {
                    valueToAdd *= hpIncreaseBoost.Value;
                }

                accessor.SetComponentData(context.InstigatorPawn, new MaximumInt<Health>() { Value = hp.Value + fix.RoundToInt(valueToAdd) });
            }
        }
    }

    public override void Unequip(ISimWorldReadWriteAccessor accessor, ItemContext context)
    {
        if (accessor.TryGetComponentData(context.ItemEntity, out ItemPassiveEffectHealthIncreaseData healthIncreaseData))
        {
            if (accessor.TryGetComponentData(context.InstigatorPawn, out Health hp))
            {
                fix valueToAdd = healthIncreaseData.Value;
                if (accessor.TryGetComponentData(context.InstigatorPawn, out HealthIncreaseMultiplier hpIncreaseBoost))
                {
                    valueToAdd *= hpIncreaseBoost.Value;
                }

                accessor.SetComponentData(context.InstigatorPawn, new MaximumInt<Health>() { Value = hp.Value + fix.RoundToInt(valueToAdd) });
            }
        }
    }

    public override void OnPawnIntStatChanged(ISimWorldReadWriteAccessor accessor, ItemContext context, IStatInt Stat) 
    {
        // if HealthIncreaseMultiplier is changed or removed, update the amount of health that is increased
        if (Stat is HealthIncreaseMultiplier)
        {
            // maybe this can be automatic if we're always doing that

            // undo
            Unequip(accessor, context);

            // redo
            Equip(accessor, context);
        }
    }
}