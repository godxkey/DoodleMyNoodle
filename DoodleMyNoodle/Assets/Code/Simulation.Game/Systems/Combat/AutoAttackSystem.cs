using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using Unity.Collections;
using UnityEngineX;

public struct AutoAttackRate : IComponentData
{
    public fix Value;

    public static implicit operator fix(AutoAttackRate val) => val.Value;
    public static implicit operator AutoAttackRate(fix val) => new AutoAttackRate() { Value = val };
}

public struct AutoAttackProgress : IComponentData
{
    public fix Value;

    public static implicit operator fix(AutoAttackProgress val) => val.Value;
    public static implicit operator AutoAttackProgress(fix val) => new AutoAttackProgress() { Value = val };
}

public struct ShouldAutoAttack : IComponentData
{
    public bool Value;

    public static implicit operator bool(ShouldAutoAttack val) => val.Value;
    public static implicit operator ShouldAutoAttack(bool val) => new ShouldAutoAttack() { Value = val };
}

public struct AutoAttackAction : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(AutoAttackAction val) => val.Value;
    public static implicit operator AutoAttackAction(Entity val) => new AutoAttackAction() { Value = val };
}

public class UpdateAutoAttackSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        var attackingEntities = GetSingletonBuffer<SystemRequestAutoAttack>();
        fix deltaTime = Time.DeltaTime;
        Entities
            .ForEach((Entity entity, ref AutoAttackProgress attackProgress, in AutoAttackRate attackRate, in ShouldAutoAttack shouldAttack) =>
            {
                attackProgress.Value = shouldAttack
                    ? attackProgress.Value + attackRate * deltaTime
                    : 0;

                if (attackProgress.Value >= 1)
                {
                    attackingEntities.Add(new SystemRequestAutoAttack()
                    {
                        Instigator = entity
                    });
                    attackProgress.Value--;
                }
            }).Run();
    }
}

public struct SystemRequestAutoAttack : ISingletonBufferElementData
{
    public Entity Instigator;
}

public class PerformAutoAttackSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        var attackingEntitiesBuffer = GetSingletonBuffer<SystemRequestAutoAttack>();
        if (attackingEntitiesBuffer.Length > 0)
        {
            NativeArray<SystemRequestAutoAttack> attackingEntities = attackingEntitiesBuffer.ToNativeArray(Allocator.Temp);
            attackingEntitiesBuffer.Clear();

            foreach (SystemRequestAutoAttack request in attackingEntities)
            {
                if (!HasComponent<AutoAttackAction>(request.Instigator))
                    continue;

                var attackAction = GetComponent<AutoAttackAction>(request.Instigator);
                CommonWrites.ExecuteGameAction(Accessor, request.Instigator, attackAction);
            }
        }
    }
}