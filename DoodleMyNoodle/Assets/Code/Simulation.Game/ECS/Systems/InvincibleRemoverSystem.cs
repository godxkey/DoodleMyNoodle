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
            Entities
            .ForEach((Entity entity, ref Invincible invincible) =>
            {
                if(invincible.Duration <= 1)
                {
                    Accessor.RemoveComponent<Invincible>(entity);
                }
                else
                {
                    Accessor.SetComponentData(entity, new Invincible() { Duration = invincible.Duration - 1 });
                }
            });
        }
    }
}
