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

public struct RemainingAutoAttackCount : IComponentData
{
    public int Value;

    public static implicit operator int(RemainingAutoAttackCount val) => val.Value;
    public static implicit operator RemainingAutoAttackCount(int val) => new RemainingAutoAttackCount() { Value = val };
}

public struct ProgressAutoAttackInAdvance : IComponentData
{
    public bool Value;

    public static implicit operator bool(ProgressAutoAttackInAdvance val) => val.Value;
    public static implicit operator ProgressAutoAttackInAdvance(bool val) => new ProgressAutoAttackInAdvance() { Value = val };
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

[UpdateBefore(typeof(ExecuteGameActionSystem))]
public class UpdateAutoAttackSystem : SimGameSystemBase
{
    private ExecuteGameActionSystem _executeGameActionSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _executeGameActionSystem = World.GetOrCreateSystem<ExecuteGameActionSystem>();
    }

    protected override void OnUpdate()
    {
        var attackingEntities = _executeGameActionSystem.CreateRequestBuffer();
        fix deltaTime = Time.DeltaTime;
        Entities
            .ForEach((Entity entity, ref AutoAttackProgress attackProgress, ref RemainingAutoAttackCount remainingAttacks,
                in AutoAttackRate attackRate, in ShouldAutoAttack shouldAttack, in ProgressAutoAttackInAdvance progressAutoAttackInAdvance, in AutoAttackAction autoAttackAction) =>
            {
                bool canAutoAttack = shouldAttack && (remainingAttacks > 0 || remainingAttacks == -1);

                attackProgress.Value = (progressAutoAttackInAdvance || canAutoAttack)
                    ? attackProgress.Value + attackRate * deltaTime
                    : 0;

                if (attackProgress.Value >= 1)
                {
                    if (canAutoAttack)
                    {
                        attackingEntities.Add(new GameActionRequest()
                        {
                            ActionEntity = autoAttackAction.Value,
                            Target = Entity.Null,
                            Instigator = entity
                        });
                        attackProgress.Value--;

                        if (remainingAttacks != -1)
                            remainingAttacks.Value--;
                    }
                    else
                    {
                        attackProgress.Value = 1; // cap at 1
                    }
                }
            }).Run();
    }
}