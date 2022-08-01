using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;

public partial class RegenResourceSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var deltaTime = Time.DeltaTime;

        Entities
            .WithNone<DeadTag>()
            .ForEach((ref Health health, in HealthMax maxHealth, in HealthRechargeCooldown cooldown, in HealthRechargeRate rechargeRate, in HealthLastHitTime lastHitTime) =>
            {
                fix elapsedTimeSinceLastHit = time - lastHitTime.Value;
                if (elapsedTimeSinceLastHit > cooldown.Value && rechargeRate.Value > 0)
                {
                    health.Value = min(maxHealth, health.Value + (rechargeRate * deltaTime));
                }
            }).Schedule();

        Entities
            .WithNone<DeadTag>()
            .ForEach((ref Shield shield, in ShieldMax maxShield, in ShieldRechargeCooldown cooldown, in ShieldRechargeRate rechargeRate, in ShieldLastHitTime lastHitTime) =>
            {
                fix elapsedTimeSinceLastHit = time - lastHitTime.Value;
                if (elapsedTimeSinceLastHit > cooldown.Value && rechargeRate.Value > 0)
                {
                    shield.Value = min(maxShield, shield.Value + (rechargeRate * deltaTime));
                }
            }).Schedule();
    }
}