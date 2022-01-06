using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System;

public static partial class CommonReads
{
    public static class Physics
    {
        public static Entity FindFirstEntityWithComponentAtPosition<T>(ISimWorldReadWriteAccessor accessor, fix2 position, Entity ignoreEntity = default) where T : struct
        {
            Entity closestEntity = Entity.Null;
            fix closestDistanceSq = 9999;

            var hits = OverlapPoint(accessor, position, ignoreEntity);
            for (int i = 0; i < hits.Length; i++)
            {
                fix dist = fixMath.distancesq((fix2)hits[i].Position, position);
                if (dist < closestDistanceSq && accessor.HasComponent<T>(hits[i].Entity))
                {
                    closestDistanceSq = dist;
                    closestEntity = hits[i].Entity;
                }
            }

            return closestEntity;
        }

        public static NativeList<OverlapPointHit> OverlapPoint(ISimWorldReadWriteAccessor accessor, fix2 position, Entity ignoreEntity = default)
        {
            NativeList<OverlapPointHit> outHits = new NativeList<OverlapPointHit>(Allocator.Temp);

            OverlapPoint(accessor, position, outHits, ignoreEntity);

            return outHits;
        }

        public static bool OverlapPoint(ISimWorldReadWriteAccessor accessor, fix2 position, NativeList<OverlapPointHit> outHits, Entity ignoreEntity = default)
        {
            var physicsSystem = accessor.GetExistingSystem<PhysicsWorldSystem>();

            OverlapPointInput pointDistanceInput = OverlapPointInput.Default;
            pointDistanceInput.Position = (float2)position;

            if (ignoreEntity != Entity.Null)
                pointDistanceInput.Ignore = new IgnoreHit(physicsSystem.GetPhysicsBodyIndex(ignoreEntity));

            return physicsSystem.PhysicsWorld.OverlapPoint(pointDistanceInput, ref outHits);
        }

        public static bool OverlapAabb(ISimWorldReadWriteAccessor accessor, fix2 min, fix2 max, NativeList<Entity> outEntities, Entity ignoreEntity = default)
        {
            var physicsSystem = accessor.GetExistingSystem<PhysicsWorldSystem>();

            OverlapAabbInput input = OverlapAabbInput.Default;
            input.Aabb = new Aabb((float2)min, (float2)max);

            if (ignoreEntity != Entity.Null)
                input.Ignore = new IgnoreHit(physicsSystem.GetPhysicsBodyIndex(ignoreEntity));

            NativeList<int> outBodyIndexes = new NativeList<int>(Allocator.Temp);
            bool hit = physicsSystem.PhysicsWorld.OverlapAabb(input, outBodyIndexes);

            for (int i = 0; i < outBodyIndexes.Length; i++)
            {
                outEntities.Add(physicsSystem.PhysicsWorld.AllBodies[outBodyIndexes[i]].Entity);
            }

            return hit;
        }

        public static NativeList<Entity> OverlapAabb(ISimWorldReadWriteAccessor accessor, fix2 min, fix2 max, Entity ignoreEntity = default)
        {
            NativeList<Entity> outHits = new NativeList<Entity>(Allocator.Temp);

            OverlapAabb(accessor, min, max, outHits, ignoreEntity);

            return outHits;
        }

        public static bool OverlapCircle(ISimWorldReadWriteAccessor accessor, fix2 position, fix radius, NativeList<DistanceHit> outHits, Entity ignoreEntity = default)
        {
            var physicsSystem = accessor.GetExistingSystem<PhysicsWorldSystem>();

            PointDistanceInput pointDistanceInput = PointDistanceInput.Default;
            pointDistanceInput.MaxDistance = (float)radius;
            pointDistanceInput.Position = (float2)position;

            if (ignoreEntity != Entity.Null)
                pointDistanceInput.Ignore = new IgnoreHit(physicsSystem.GetPhysicsBodyIndex(ignoreEntity));

            return physicsSystem.PhysicsWorld.CalculateDistance(pointDistanceInput, ref outHits);
        }

        public static NativeList<DistanceHit> OverlapCircle(ISimWorldReadWriteAccessor accessor, fix2 position, fix radius, Entity ignoreEntity = default)
        {
            NativeList<DistanceHit> outHits = new NativeList<DistanceHit>(Allocator.Temp);

            OverlapCircle(accessor, position, radius, outHits, ignoreEntity);

            return outHits;
        }
    }
}
