using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class DirectToDestinationSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Translation translation, ref Velocity velocity, ref Destination destination, ref MoveSpeed moveSpeed) =>
        {
            float3 deltaMove = destination.Value - translation.Value;
            float moveLength = length(deltaMove);

            if (moveLength < 0.01f)
            {
                // arrived to destination
                translation.Value = destination.Value;
                velocity.Value = float3(0);
                EntityManager.RemoveComponent<Destination>(entity);
            }
            else
            {
                // update velocity
                float3 normalVel = (deltaMove / moveLength) * moveSpeed.Value;
                float3 teleportToDestinationVelocity = deltaMove / Time.DeltaTime;

                if (lengthsq(teleportToDestinationVelocity) < lengthsq(normalVel) || any(isnan(normalVel)))
                {
                    // final smaller step, just enough velocity to reach destination
                    velocity.Value = teleportToDestinationVelocity;
                }
                else
                {
                    // normal velocity
                    velocity.Value = normalVel;
                }
            }
        });
    }
}
