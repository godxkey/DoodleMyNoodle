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
        var elapseTime = Time.ElapsedTime;

        Entities
            .ForEach((Entity entity, ref Shield shield, ref ShieldRechargeRate shieldRechargeRate, ref ShieldRechargeCooldown shieldRechargeCooldown, ref ShieldLastHitTime shieldLastHitTime) =>
            {
                fix deltaTime = elapseTime - shieldLastHitTime.Value;
                if (deltaTime > shieldRechargeCooldown.Value)
                {
                    CommonWrites.ModifyStatFix<Shield>(Accessor, entity, shieldRechargeRate);
                    shieldLastHitTime = elapseTime;
                }
            }).WithoutBurst().Run();
    }
}