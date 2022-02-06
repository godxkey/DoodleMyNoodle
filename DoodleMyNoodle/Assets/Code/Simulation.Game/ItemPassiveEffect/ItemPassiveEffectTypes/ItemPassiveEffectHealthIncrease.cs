using System;
using UnityEngine;
using UnityEngineX;

public class ItemPassiveEffectHealthIncrease : ItemPassiveEffect
{
    public override void Equip(ISimGameWorldReadWriteAccessor accessor, ItemContext context)
    {
        if (accessor.TryGetComponent(context.ItemEntity, out ItemPassiveEffectHealthIncreaseData healthIncreaseData))
        {
            if (accessor.TryGetComponent(context.InstigatorPawn, out Health hp))
            {
                fix valueToAdd = healthIncreaseData.Value;
                if (accessor.TryGetComponent(context.InstigatorPawn, out HealthIncreaseMultiplier hpIncreaseBoost))
                {
                    valueToAdd *= hpIncreaseBoost.Value;
                }

                accessor.SetComponent(context.InstigatorPawn, new MaximumInt<Health>() { Value = hp.Value + fix.RoundToInt(valueToAdd) });
            }
        }
    }

    public override void Unequip(ISimGameWorldReadWriteAccessor accessor, ItemContext context)
    {
        if (accessor.TryGetComponent(context.ItemEntity, out ItemPassiveEffectHealthIncreaseData healthIncreaseData))
        {
            if (accessor.TryGetComponent(context.InstigatorPawn, out Health hp))
            {
                fix valueToAdd = healthIncreaseData.Value;
                if (accessor.TryGetComponent(context.InstigatorPawn, out HealthIncreaseMultiplier hpIncreaseBoost))
                {
                    valueToAdd *= hpIncreaseBoost.Value;
                }

                accessor.SetComponent(context.InstigatorPawn, new MaximumInt<Health>() { Value = hp.Value + fix.RoundToInt(valueToAdd) });
            }
        }
    }

    public override void OnPawnIntStatChanged(ISimGameWorldReadWriteAccessor accessor, ItemContext context, IStatInt Stat) 
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