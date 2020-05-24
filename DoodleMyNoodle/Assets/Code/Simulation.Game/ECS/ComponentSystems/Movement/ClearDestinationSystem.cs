using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using static Unity.Mathematics.math;
using static fixMath;

[UpdateAfter(typeof(ApplyVelocitySystem))]
public class ClearDestinationSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity,
            ref FixTranslation translation,
            ref Destination destination) =>
        {
            fix3 deltaMove = destination.Value - translation.Value;
            fix moveLengthSq = lengthsq(deltaMove);

            if (moveLengthSq < fix(0.0001f))
            {
                // arrived to destination
                translation.Value = destination.Value;
                EntityManager.RemoveComponent<Destination>(entity);
            }
        });

        //Entities.ForEach((Entity entity,
        //ref FixTranslation translation,
        //ref PotentialNewTranslation potentialNewTranslation,
        //ref Destination destination) =>
        //{
        //    if(potentialNewTranslation.Value == translation.Value)
        //    {
        //        destination.Value = new Destination() { Value = fix3(currentTilePos, 0) });
        //    }
        //});
    }       
}
