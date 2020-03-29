using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using static Unity.Mathematics.math;
using static FixMath;

public class DirectToDestinationSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity,
            ref FixTranslation translation, 
            ref Velocity velocity, 
            ref Destination destination, 
            ref MoveSpeed moveSpeed) =>
        {
            FixVector3 deltaMove = destination.Value - translation.Value;
            Fix64 moveLength = length(deltaMove);

            if (moveLength < fix(0.01f))
            {
                // arrived to destination
                translation.Value = destination.Value;
                velocity.Value = fix3(0);
                EntityManager.RemoveComponent<Destination>(entity);
            }
            else
            {
                // update velocity
                FixVector3 normalVel = (deltaMove / moveLength) * moveSpeed.Value;
                FixVector3 teleportToDestinationVelocity = deltaMove / Time.DeltaTime;

                if (lengthsq(teleportToDestinationVelocity) < lengthsq(normalVel))
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
