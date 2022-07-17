using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Entities;
using static fixMath;
using static Unity.Mathematics.math;


public struct SystemRequestImpulseDirect : ISingletonBufferElementData
{
    public Entity Target;
    public fix2 Strength;
    public bool IgnoreMass;
}

public struct SystemRequestImpulseRadial : ISingletonBufferElementData
{
    public Entity Target;
    public fix StrengthMin;
    public fix StrengthMax;
    public fix Radius;
    public fix2 Position;
    public bool IgnoreMass;
}

[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateBefore(typeof(UpdateCanMoveSystem))]
public partial class ApplyImpulseSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        HandleRadialImpulseRequests();
        HandleDirectImpulseRequests();
    }

    private void HandleRadialImpulseRequests()
    {
        DynamicBuffer<SystemRequestImpulseRadial> radialImpulses = GetSingletonBuffer<SystemRequestImpulseRadial>();

        if (radialImpulses.Length > 0)
        {
            DynamicBuffer<SystemRequestImpulseDirect> directImpulseRequests = GetSingletonBuffer<SystemRequestImpulseDirect>();

            foreach (SystemRequestImpulseRadial request in radialImpulses)
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

                fix2 direction = distance < fix.Epsilon ? new fix2(0, 1) : v / distance;

                directImpulseRequests.Add(new SystemRequestImpulseDirect()
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
        DynamicBuffer<SystemRequestImpulseDirect> directImpulseRequests = GetSingletonBuffer<SystemRequestImpulseDirect>();

        if (directImpulseRequests.Length > 0)
        {
            foreach (SystemRequestImpulseDirect request in directImpulseRequests)
            {
                if (!HasComponent<PhysicsVelocity>(request.Target))
                    continue;

                if (!HasComponent<PhysicsMass>(request.Target))
                    continue;

                var mass = GetComponent<PhysicsMass>(request.Target);

                PhysicsVelocity vel = GetComponent<PhysicsVelocity>(request.Target);
                vel.Linear += request.Strength * (request.IgnoreMass ? 1 : (fix)mass.InverseMass);
                SetComponent(request.Target, vel);

                if (HasComponent<Grounded>(request.Target))
                {
                    SetComponent<Grounded>(request.Target, false);
                }
            }

            directImpulseRequests.Clear();
        }
    }
}

internal static partial class CommonWrites
{
    public static void RequestRadialImpulse(ISimGameWorldReadWriteAccessor accessor, Entity target, fix strengthMin, fix strengthMax, fix radius, fix2 position, bool ignoreMass = false)
    {
        SystemRequestImpulseRadial request = new SystemRequestImpulseRadial()
        {
            Target = target,
            Radius = radius,
            Position = position,
            IgnoreMass = ignoreMass,
            StrengthMax = strengthMax,
            StrengthMin = strengthMin,
        };

        accessor.GetSingletonBuffer<SystemRequestImpulseRadial>().Add(request);
    }

    public static void RequestImpulse(ISimGameWorldReadWriteAccessor accessor, Entity target, fix2 strength, bool ignoreMass = false)
    {
        SystemRequestImpulseDirect request = new SystemRequestImpulseDirect()
        {
            Target = target,
            Strength = strength,
            IgnoreMass = ignoreMass,
        };

        accessor.GetSingletonBuffer<SystemRequestImpulseDirect>().Add(request);
    }
}
