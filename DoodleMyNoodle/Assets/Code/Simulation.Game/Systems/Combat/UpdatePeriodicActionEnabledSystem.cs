using Unity.Entities;
using CCC.Fix2D;

public struct PeriodicActionRange : IComponentData
{
    public FixRange Value;

    public static implicit operator FixRange(PeriodicActionRange val) => val.Value;
    public static implicit operator PeriodicActionRange(FixRange val) => new PeriodicActionRange() { Value = val };
}

[UpdateAfter(typeof(MovementSystemGroup))]
public partial class UpdatePeriodicActionEnabledSystem : SimGameSystemBase
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
        var offsetFromTargets = GetComponentDataFromEntity<OffsetFromTarget>(isReadOnly: true);
        Entities
            .WithReadOnly(healths)
            .WithReadOnly(offsetFromTargets)
            .WithAll<ItemTag>()
            .ForEach((ref PeriodicActionEnabled periodicEnabled,
                      in FirstInstigator owner,
                      in PeriodicActionRange range) =>
            {
                bool ownerHPOk = !healths.TryGetComponent(owner, out var hp) || hp.Value > 0;
                bool ownerDistanceOk = !offsetFromTargets.TryGetComponent(owner, out var offsetFromTarget)
                    || range.Value.Contains(offsetFromTarget, epsilon: (fix)0.01f);

                periodicEnabled = ownerHPOk && ownerDistanceOk;
            }).Schedule();

        // _________________________________________ Mobs _________________________________________ //
        Entities
            .ForEach((ref PeriodicActionEnabled periodicEnabled,
                      in OffsetFromTarget offsetFromTarget,
                      in Health hp,
                      in PeriodicActionRange range) =>
        {
            periodicEnabled = hp.Value > 0
                && range.Value.Contains(offsetFromTarget, epsilon: (fix)0.01f);
        }).Schedule();
    }
}
