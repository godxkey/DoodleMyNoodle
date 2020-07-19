using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct DamageToApplySingletonTag : IComponentData
{
}

public struct DamageToApplyData : IBufferElementData
{
    public Entity Instigator;
    public int Amount;
    public Entity Target;
}

public class ApplyDamageSystem : SimComponentSystem
{
    public static DynamicBuffer<DamageToApplyData> GetDamageToApplySingletonBuffer(ISimWorldReadWriteAccessor accessor)
    {
        if (!accessor.HasSingleton<DamageToApplySingletonTag>())
        {
            accessor.CreateEntity(typeof(DamageToApplySingletonTag), typeof(DamageToApplyData));
        }

        return accessor.GetBuffer<DamageToApplyData>(accessor.GetSingletonEntity<DamageToApplySingletonTag>());
    }

    protected override void OnUpdate()
    {
        DynamicBuffer<DamageToApplyData> DamageToApplyBuffer = GetDamageToApplySingletonBuffer(Accessor);

        foreach (DamageToApplyData damageData in DamageToApplyBuffer)
        {
            int remainingDamage = damageData.Amount;
            Entity target = damageData.Target;

            // Invincible
            if (Accessor.HasComponent<Invincible>(target))
            {
                remainingDamage = 0;
            }

            // Armor
            if (remainingDamage > 0 && Accessor.TryGetComponentData(target, out Armor armor))
            {
                CommonWrites.ModifyStatInt<Armor>(Accessor, target, -remainingDamage);
                remainingDamage -= armor.Value;
            }

            // Health
            if (remainingDamage > 0 && Accessor.TryGetComponentData(target, out Health health))
            {
                CommonWrites.ModifyStatInt<Health>(Accessor, target, -remainingDamage);
                remainingDamage -= health.Value;
            }
        }

        DamageToApplyBuffer.Clear();
    }
}

internal static partial class CommonWrites
{
    public static void RequestDamageOnTarget(ISimWorldReadWriteAccessor accessor, Entity instigator, Entity target, int amount)
    {
        DynamicBuffer<DamageToApplyData> damageDataBuffer = ApplyDamageSystem.GetDamageToApplySingletonBuffer(accessor);

        damageDataBuffer.Add(new DamageToApplyData() { Amount = amount, Instigator = instigator, Target = target });
    }
}