public class ItemPassiveEffectHealthIncreaseMultiplier : ItemPassiveEffect
{
    public override void Equip(ISimGameWorldReadWriteAccessor accessor, ItemContext context)
    {
        if (accessor.TryGetComponent(context.ItemEntity, out ItemPassiveEffectHealthIncreaseMultiplierData healthIncreaseBoostData))
        {
            if (accessor.HasComponent<HealthIncreaseMultiplier>(context.InstigatorPawn))
            {
                CommonWrites.ModifyStatInt<HealthIncreaseMultiplier>(accessor, context.InstigatorPawn, healthIncreaseBoostData.Value);
            }
            else
            {
                accessor.AddComponent(context.InstigatorPawn, new HealthIncreaseMultiplier() { Value = healthIncreaseBoostData.Value });
            }
        }
    }

    public override void Unequip(ISimGameWorldReadWriteAccessor accessor, ItemContext context)
    {
        if (accessor.TryGetComponent(context.ItemEntity, out ItemPassiveEffectHealthIncreaseMultiplierData healthIncreaseBoostData))
        {
            if (accessor.HasComponent<HealthIncreaseMultiplier>(context.InstigatorPawn))
            {
                CommonWrites.ModifyStatInt<HealthIncreaseMultiplier>(accessor, context.InstigatorPawn, -healthIncreaseBoostData.Value);
            }
        }
    }
}