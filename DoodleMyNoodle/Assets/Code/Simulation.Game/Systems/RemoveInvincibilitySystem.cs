using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

// ce system devrait �tre retir� lorsqu'on aura un system de buff/debuff avec dur�e

public class RemoveInvincibilitySystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewRoundEventData>())
        {
            Entities.ForEach((Entity entity, ref Invincible invincible) =>
            {
                invincible.Duration--;

                if (invincible.Duration <= 0)
                {
                    EntityManager.RemoveComponent<Invincible>(entity);
                }
            }).WithoutBurst()
            .WithStructuralChanges()
            .Run();
        }
    }
}
