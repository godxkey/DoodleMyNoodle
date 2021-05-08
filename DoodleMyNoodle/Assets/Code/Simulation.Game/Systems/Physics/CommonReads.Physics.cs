using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public static partial class CommonReads
{
    public static class Physics
    {
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

        public static NativeList<DistanceHit> OverlapCircle(ISimWorldReadWriteAccessor accessor, fix2 attackPosition, fix attackRadius, Entity ignoreEntity = default)
        {
            NativeList<DistanceHit> outHits = new NativeList<DistanceHit>(Allocator.Temp);

            OverlapCircle(accessor, attackPosition, attackRadius, outHits, ignoreEntity);

            return outHits;
        }
    }
}
