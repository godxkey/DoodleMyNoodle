using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

// ce system devrait être retiré lorsqu'on aura un system de buff/debuff avec durée

public class RemoveInvincibilitySystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            int currentTeam = CommonReads.GetTurnTeam(Accessor);

            Entities.ForEach((Entity entity, ref Invincible invincible, in Controllable controller) =>
            {
                if (TryGetComponent(controller, out Team team))
                {
                    if (team == currentTeam)
                    {
                        invincible.Duration--;
                    }

                    if (invincible.Duration <= 0)
                    {
                        EntityManager.RemoveComponent<Invincible>(entity);
                    }
                }
            }).WithoutBurst()
            .WithStructuralChanges()
            .Run();
        }
    }
}
