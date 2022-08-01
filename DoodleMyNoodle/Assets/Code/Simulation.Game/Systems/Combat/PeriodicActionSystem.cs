using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using Unity.Collections;
using UnityEngineX;

public struct PeriodicActionRate : IComponentData
{
    public fix Value;
    public bool ScaleWithOwnerAttackSpeed;

    public static implicit operator fix(PeriodicActionRate val) => val.Value;
    public static implicit operator PeriodicActionRate(fix val) => new PeriodicActionRate() { Value = val };
}

public struct PeriodicActionCount : IComponentData
{
    public int Value;

    public static implicit operator int(PeriodicActionCount val) => val.Value;
    public static implicit operator PeriodicActionCount(int val) => new PeriodicActionCount() { Value = val };
}

public struct RemainingPeriodicActionCount : IComponentData
{
    public int Value;

    public static implicit operator int(RemainingPeriodicActionCount val) => val.Value;
    public static implicit operator RemainingPeriodicActionCount(int val) => new RemainingPeriodicActionCount() { Value = val };
}

public struct ProgressPeriodicActionInAdvance : IComponentData
{
    public bool Value;

    public static implicit operator bool(ProgressPeriodicActionInAdvance val) => val.Value;
    public static implicit operator ProgressPeriodicActionInAdvance(bool val) => new ProgressPeriodicActionInAdvance() { Value = val };
}

public struct PeriodicActionProgress : IComponentData
{
    public fix Value;

    public static implicit operator fix(PeriodicActionProgress val) => val.Value;
    public static implicit operator PeriodicActionProgress(fix val) => new PeriodicActionProgress() { Value = val };
}

public struct PeriodicActionEnabled : IComponentData
{
    public bool Value;

    public static implicit operator bool(PeriodicActionEnabled val) => val.Value;
    public static implicit operator PeriodicActionEnabled(bool val) => new PeriodicActionEnabled() { Value = val };
}

public struct PeriodicAction : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(PeriodicAction val) => val.Value;
    public static implicit operator PeriodicAction(Entity val) => new PeriodicAction() { Value = val };
}

[UpdateBefore(typeof(ExecuteGameActionSystem))]
public partial class UpdatePeriodicActionSystem : SimGameSystemBase
{
    private ExecuteGameActionSystem _executeGameActionSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _executeGameActionSystem = World.GetOrCreateSystem<ExecuteGameActionSystem>();
    }

    protected override void OnUpdate()
    {
        var actingEntities = _executeGameActionSystem.CreateRequestBuffer();
        fix deltaTime = Time.DeltaTime;
        Entities
            .ForEach((Entity entity, ref PeriodicActionProgress progress, ref RemainingPeriodicActionCount remainingCount,
                in PeriodicActionRate rate, in PeriodicActionEnabled progressEnabled, in ProgressPeriodicActionInAdvance progressInAdvance, in PeriodicAction action) =>
            {
                bool canProgress = progressEnabled && (remainingCount > 0 || remainingCount == -1);

                // ATTACK SPEED
                fix totalRate = rate;
                if (HasComponent<AttackSpeed>(entity))
                {
                    totalRate *= GetComponent<AttackSpeed>(entity).Value;
                }

                if (rate.ScaleWithOwnerAttackSpeed && HasComponent<Owner>(entity))
                {
                    var owner = GetComponent<Owner>(entity);
                    if (HasComponent<AttackSpeed>(owner))
                    {
                        totalRate *= GetComponent<AttackSpeed>(owner).Value;
                    }
                }

                progress.Value = (progressInAdvance || canProgress)
                    ? progress.Value + totalRate * deltaTime
                    : 0;

                if (progress.Value >= 1)
                {
                    if (canProgress)
                    {
                        actingEntities.Add(new GameActionRequest()
                        {
                            ActionEntity = action,
                            Target = Entity.Null,
                            Instigator = entity
                        });
                        progress.Value--;

                        if (remainingCount != -1)
                            remainingCount.Value--;
                    }
                    else
                    {
                        progress.Value = 1; // cap at 1
                    }
                }
            }).Run();
    }
}