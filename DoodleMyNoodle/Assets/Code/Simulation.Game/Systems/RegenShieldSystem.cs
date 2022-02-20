using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;

public class RegenShieldSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var deltaTime = Time.DeltaTime;

        Entities
            .ForEach((Entity entity, ref Shield shield, in ShieldRechargeCooldown cooldown, in ShieldRechargeRate rechargeRate, in ShieldLastHitTime lastHitTime) =>
            {
                fix elapsedTimeSinceLastHit = time - lastHitTime.Value;
                if (elapsedTimeSinceLastHit > cooldown.Value)
                {
                    shield.Value = min(GetComponent<MaximumFix<Shield>>(entity), shield.Value + (rechargeRate * deltaTime));
                }
            }).Run();
    }
}