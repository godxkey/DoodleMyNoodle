using Unity.Entities;
using CCC.Fix2D;

public struct MeleeAttackerTag : IComponentData { }
public struct DropAttackerTag : IComponentData { }

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

        // _________________________________________ Player Attacker _________________________________________ //
        Entities
            .WithAll<ItemTag>()
            .ForEach((ref PeriodicActionEnabled periodicEnabled) =>
            {
                periodicEnabled = true;
            }).Schedule();

        // _________________________________________ Melee Attacker _________________________________________ //
        Entities
            .WithAll<MeleeAttackerTag>()
            .ForEach((ref PeriodicActionEnabled periodicEnabled, in CanMove canMove, in Health hp) =>
        {
            periodicEnabled = !canMove && hp.Value > 0;
        }).Schedule();

        // _________________________________________ Drop Attacker _________________________________________ //
        fix2 playerGroupPosition = GetComponent<FixTranslation>(GetSingletonEntity<PlayerGroupDataTag>());
        Entities
            .WithAll<DropAttackerTag>()
            .ForEach((ref PeriodicActionEnabled periodicEnabled, in FixTranslation position, in Health hp) =>
            {
                periodicEnabled = position.Value.x < playerGroupPosition.x && hp.Value > 0;
            }).Schedule();

        // _________________________________________ Frozen _________________________________________ //
        Entities
            .WithAll<Frozen>()
            .ForEach((ref PeriodicActionEnabled periodicEnabled) =>
            {
                periodicEnabled = false;
            }).Schedule();
    }
}
