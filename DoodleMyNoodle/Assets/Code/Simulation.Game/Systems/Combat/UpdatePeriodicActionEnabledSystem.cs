using Unity.Entities;
using CCC.Fix2D;

public struct PeriodicActionDistanceMin : IComponentData
{
    public fix Value;

    public static implicit operator fix(PeriodicActionDistanceMin val) => val.Value;
    public static implicit operator PeriodicActionDistanceMin(fix val) => new PeriodicActionDistanceMin() { Value = val };
}

public struct PeriodicActionDistanceMax : IComponentData
{
    public fix Value;

    public static implicit operator fix(PeriodicActionDistanceMax val) => val.Value;
    public static implicit operator PeriodicActionDistanceMax(fix val) => new PeriodicActionDistanceMax() { Value = val };
}

public class UpdateShouldAutoAttackSystem : SimGameSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<PlayerGroupDataTag>();
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<GameStartedTag>())
        {
            Entities
                .ForEach((ref PeriodicActionEnabled periodicEnabled) =>
                {
                    periodicEnabled = false;
                }).Run();
            return;
        }

        // _________________________________________ Items _________________________________________ //
        var healths = GetComponentDataFromEntity<Health>(isReadOnly: true);
        var distancesFromTargets = GetComponentDataFromEntity<DistanceFromTarget>(isReadOnly: true);
        Entities
            .WithReadOnly(healths)
            .WithReadOnly(distancesFromTargets)
            .WithAll<ItemTag>()
            .ForEach((ref PeriodicActionEnabled periodicEnabled,
                      in FirstInstigator owner,
                      in PeriodicActionDistanceMin distMin,
                      in PeriodicActionDistanceMax distMax) =>
            {
                bool ownerHPOk = !healths.TryGetComponent(owner, out var hp) || hp.Value > 0;
                bool ownerDistanceOk = !distancesFromTargets.TryGetComponent(owner, out var distanceFromTarget)
                    || (distMin <= distanceFromTarget.Value && distanceFromTarget.Value <= distMax);

                periodicEnabled = ownerHPOk && ownerDistanceOk;
            }).Schedule();

        // _________________________________________ Mobs _________________________________________ //
        Entities
            .ForEach((ref PeriodicActionEnabled periodicEnabled,
                      in DistanceFromTarget distanceFromTarget,
                      in Health hp,
                      in PeriodicActionDistanceMin distMin,
                      in PeriodicActionDistanceMax distMax) =>
        {
            periodicEnabled = hp.Value > 0
                && distanceFromTarget.Value >= distMin
                && distanceFromTarget.Value <= distMax;
        }).Schedule();
    }
}
