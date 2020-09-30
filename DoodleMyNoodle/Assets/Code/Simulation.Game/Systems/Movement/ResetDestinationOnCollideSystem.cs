﻿using Unity.Entities;

[UpdateAfter(typeof(ApplyPotentialNewTranslationSystem))]
[UpdateInGroup(typeof(MovementSystemGroup))]
public class ResetDestinationOnCollideSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // if entities collide when moving along path, reset their destination
        Entities.ForEach((ref TileCollisionEventData collisionEvent) =>
        {
            if (EntityManager.HasComponent<PathPosition>(collisionEvent.Entity))
            {
                fix3 entityPos = EntityManager.GetComponentData<FixTranslation>(collisionEvent.Entity).Value;
                fix3 newDestination = Helpers.GetTileCenter(entityPos);
                EntityManager.SetOrAddComponentData(collisionEvent.Entity, new Destination() { Value = newDestination });
            }
        });
    }
}