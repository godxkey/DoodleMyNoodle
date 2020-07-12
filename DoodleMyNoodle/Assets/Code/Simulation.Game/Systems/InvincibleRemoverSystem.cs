using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

public class InvincibleRemoverSystem : SimComponentSystem
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
