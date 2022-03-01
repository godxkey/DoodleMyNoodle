using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;

public class RegenHealthAndShieldSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var deltaTime = Time.DeltaTime;

        Entities
            .WithNone<DeadTag>()
            .ForEach((Entity entity, ref Health health, in HealthRechargeCooldown cooldown, in HealthRechargeRate rechargeRate, in HealthLastHitTime lastHitTime) =>
            {
                fix elapsedTimeSinceLastHit = time - lastHitTime.Value;
                if (elapsedTimeSinceLastHit > cooldown.Value && rechargeRate.Value > 0)
                {
                    health.Value = min(GetComponent<MaximumFix<Health>>(entity), health.Value + (rechargeRate * deltaTime));
                }
            }).Run();

        Entities
            .WithNone<DeadTag>()
            .ForEach((Entity entity, ref Shield shield, in ShieldRechargeCooldown cooldown, in ShieldRechargeRate rechargeRate, in ShieldLastHitTime lastHitTime) =>
            {
                fix elapsedTimeSinceLastHit = time - lastHitTime.Value;
                if (elapsedTimeSinceLastHit > cooldown.Value && rechargeRate.Value > 0)
                {
                    shield.Value = min(GetComponent<MaximumFix<Shield>>(entity), shield.Value + (rechargeRate * deltaTime));
                }
            }).Run();
    }
}