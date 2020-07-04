using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct DamageToApplyDataContainer : IComponentData
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
    protected override void OnCreate()
    {
        base.OnCreate();

        Accessor.SetOrCreateSingleton(new DamageToApplyDataContainer());
        Accessor.AddBuffer<DamageToApplyData>(Accessor.GetSingletonEntity<DamageToApplyDataContainer>());
    }

    protected override void OnUpdate()
    {
        Accessor.Entities.ForEach((Entity singleton, ref DamageToApplyDataContainer containerTag, DynamicBuffer<DamageToApplyData> readyForNextTurn) =>
        {
            foreach (DamageToApplyData damageData in readyForNextTurn)
            {
                int realDamageToApply = damageData.Amount;
                Entity target = damageData.Target;
                if (!Accessor.HasComponent<Invincible>(target))
                {
                    if (Accessor.TryGetComponentData(target, out Armor armor))
                    {
                        int newArmorValue = armor.Value - realDamageToApply;
                        if(newArmorValue < 0)
                        {
                            realDamageToApply = math.abs(newArmorValue);
                        }

                        CommonWrites.SetStatInt(Accessor, target, new Armor() { Value = math.max(newArmorValue, 0) });
                    }

                    CommonWrites.ModifyStatInt<Health>(Accessor, target, -realDamageToApply);
                }
            }

            readyForNextTurn.Clear();
        });
    }
}

internal static partial class CommonWrites
{
    public static void RequestDamageOnTarget(ISimWorldReadWriteAccessor accessor, Entity instigator, Entity target, int amount)
    {
        Entity container = accessor.GetSingletonEntity<DamageToApplyDataContainer>();
        DynamicBuffer<DamageToApplyData> damageDataBuffer = accessor.GetBuffer<DamageToApplyData>(container);

        damageDataBuffer.Add(new DamageToApplyData() { Amount = amount, Instigator = instigator, Target = target });
    }
}