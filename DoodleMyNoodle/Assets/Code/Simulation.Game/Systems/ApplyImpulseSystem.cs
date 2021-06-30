using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Entities;
using static fixMath;
using static Unity.Mathematics.math;

public struct ImpulseRequestSingletonTag : IComponentData { }

public struct DirectMoveRequestData : IBufferElementData
{
    public Entity Target;
    public fix2 Velocity;
}

public struct DirectImpulseRequestData : IBufferElementData
{
    public Entity Target;
    public fix2 Strength;
    public bool IgnoreMass;
}

public struct RadialImpulseRequestData : IBufferElementData
{
    public Entity Target;
    public fix StrengthMin;
    public fix StrengthMax;
    public fix Radius;
    public fix2 Position;
    public bool IgnoreMass;
}

public class ApplyImpulseSystem : SimSystemBase
{
    public void RequestImpulseRadial(RadialImpulseRequestData request)
    {
        var buffer = GetBuffer<RadialImpulseRequestData>(GetRequestSingleton());
        buffer.Add(request);
    }

    public void RequestImpulseDirect(DirectImpulseRequestData request)
    {
        var buffer = GetBuffer<DirectImpulseRequestData>(GetRequestSingleton());
        buffer.Add(request);
    }

    public void RequestMoveDirect(DirectMoveRequestData request)
    {
        var buffer = GetBuffer<DirectMoveRequestData>(GetRequestSingleton());
        buffer.Add(request);
    }

    private Entity GetRequestSingleton()
    {
        if (!HasSingleton<ImpulseRequestSingletonTag>())
        {
            return EntityManager.CreateEntity(typeof(ImpulseRequestSingletonTag), typeof(DirectImpulseRequestData), typeof(RadialImpulseRequestData), typeof(DirectMoveRequestData));
        }
        return GetSingletonEntity<ImpulseRequestSingletonTag>();
    }

    protected override void OnUpdate()
    {
        HandleRadialImpulseRequests();
        HandleDirectImpulseRequests();
        HandleDirectMoveRequests();
    }

    private void HandleRadialImpulseRequests()
    {
        DynamicBuffer<RadialImpulseRequestData> radialImpulses = GetBuffer<RadialImpulseRequestData>(GetRequestSingleton());

        if (radialImpulses.Length > 0)
        {
            DynamicBuffer<DirectImpulseRequestData> directImpulseRequests = GetBuffer<DirectImpulseRequestData>(GetRequestSingleton());

            foreach (RadialImpulseRequestData request in radialImpulses)
            {
                if (!HasComponent<PhysicsColliderBlob>(request.Target))
                    continue;

                var colliderRef = GetComponent<PhysicsColliderBlob>(request.Target).Collider;
                if (!colliderRef.IsCreated)
                    continue;

                ref Collider collider = ref colliderRef.Value;

                fix2 pos = GetComponent<FixTranslation>(request.Target);
                fix2 centerOfMass = pos + (fix2)collider.MassProperties.MassDistribution.LocalCenterOfMass;
                fix2 v = centerOfMass - request.Position;
                fix distance = length(v);
                fix impulseStrength = remap(
                    0, request.Radius,
                    request.StrengthMax, request.StrengthMin,
                    distance);

                fix2 direction = distance < fix.epsilon ? new fix2(0, 1) : v / distance;

                directImpulseRequests.Add(new DirectImpulseRequestData()
                {
                    IgnoreMass = request.IgnoreMass,
                    Strength = impulseStrength * direction,
                    Target = request.Target
                });
            }

            radialImpulses.Clear();
        }
    }

    private void HandleDirectImpulseRequests()
    {
        DynamicBuffer<DirectImpulseRequestData> directImpulseRequests = GetBuffer<DirectImpulseRequestData>(GetRequestSingleton());

        if (directImpulseRequests.Length > 0)
        {
            foreach (DirectImpulseRequestData request in directImpulseRequests)
            {
                if (!HasComponent<PhysicsVelocity>(request.Target))
                    continue;

                if (!HasComponent<PhysicsMass>(request.Target))
                    continue;

                var mass = GetComponent<PhysicsMass>(request.Target);

                PhysicsVelocity vel = GetComponent<PhysicsVelocity>(request.Target);
                vel.Linear += request.Strength * (request.IgnoreMass ? 1 : (fix)mass.InverseMass);
                SetComponent(request.Target, vel);

                if (HasComponent<NavAgentFootingState>(request.Target))
                {
                    SetComponent(request.Target, new NavAgentFootingState() { Value = NavAgentFooting.None });
                }
            }

            directImpulseRequests.Clear();
        }
    }

    private void HandleDirectMoveRequests()
    {
        DynamicBuffer<DirectMoveRequestData> directMoveRequests = GetBuffer<DirectMoveRequestData>(GetRequestSingleton());

        if (directMoveRequests.Length > 0)
        {
            foreach (DirectMoveRequestData request in directMoveRequests)
            {
                if (!HasComponent<PhysicsVelocity>(request.Target))
                    continue;

                if (!HasComponent<PhysicsMass>(request.Target))
                    continue;

                var speed = GetComponent<MoveSpeed>(request.Target);
                PhysicsVelocity vel = GetComponent<PhysicsVelocity>(request.Target);

                // vel is already above speed limit, cannot move to go faster than that
                if (vel.Linear.lengthSquared > fix.Pow(speed.Value, 2))
                    continue;

                var mass = GetComponent<PhysicsMass>(request.Target);

                fix2 currentVelLinear = vel.Linear;
                fix2 velToAdd = request.Velocity * (fix)mass.InverseMass;
                currentVelLinear += velToAdd;

                // is the movement sending us past the move speed
                if (currentVelLinear.lengthSquared > fix.Pow(speed.Value,2))
                {
                    // yes, so ajust it to reach the speed limit
                    fix diff = currentVelLinear.length - vel.Linear.length;
                    velToAdd.Normalize();
                    velToAdd *= diff;
                }

                vel.Linear += velToAdd;

                SetComponent(request.Target, vel);

                if (HasComponent<NavAgentFootingState>(request.Target))
                {
                    SetComponent(request.Target, new NavAgentFootingState() { Value = NavAgentFooting.None });
                }
            }

            directMoveRequests.Clear();
        }
    }
}

internal static partial class CommonWrites
{
    public static void RequestRadialImpulse(ISimWorldReadWriteAccessor accessor, Entity target, fix strengthMin, fix strengthMax, fix radius, fix2 position, bool ignoreMass = false)
    {
        RadialImpulseRequestData request = new RadialImpulseRequestData()
        {
            Target = target,
            Radius = radius,
            Position = position,
            IgnoreMass = ignoreMass,
            StrengthMax = strengthMax,
            StrengthMin = strengthMin,
        };

        accessor.GetExistingSystem<ApplyImpulseSystem>().RequestImpulseRadial(request);
    }

    public static void RequestImpulse(ISimWorldReadWriteAccessor accessor, Entity target, fix2 strength, bool ignoreMass = false)
    {
        DirectImpulseRequestData request = new DirectImpulseRequestData()
        {
            Target = target,
            Strength = strength,
            IgnoreMass = ignoreMass,
        };

        accessor.GetExistingSystem<ApplyImpulseSystem>().RequestImpulseDirect(request);
    }

    public static void RequestMove(ISimWorldReadWriteAccessor accessor, Entity target, fix2 velocity)
    {
        DirectMoveRequestData request = new DirectMoveRequestData()
        {
            Target = target,
            Velocity = velocity
        };

        accessor.GetExistingSystem<ApplyImpulseSystem>().RequestMoveDirect(request);
    }
}
