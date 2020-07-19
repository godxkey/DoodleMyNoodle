using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

// ce system devrait être retiré lorsqu'on aura un system de buff/debuff avec durée

public class RemoveInvincibilitySystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            int currentTeam = CommonReads.GetCurrentTurnTeam(Accessor);

            Entities.ForEach((Entity entity, ref Invincible invincible) =>
            {
                Entity pawnController = CommonReads.GetPawnController(Accessor, entity);
                if (pawnController != Entity.Null)
                {
                    if (EntityManager.GetComponentData<Team>(pawnController).Value == currentTeam)
                    {
                        invincible.Duration--;
                    }

                    if (invincible.Duration <= 0)
                    {
                        PostUpdateCommands.RemoveComponent<Invincible>(entity);
                    }
                }
            });
        }
    }
}
