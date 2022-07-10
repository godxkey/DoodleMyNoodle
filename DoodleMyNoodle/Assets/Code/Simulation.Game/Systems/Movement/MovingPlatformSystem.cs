using CCC.Fix2D;
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public enum PlatformMoveMode
{
    Yoyo,
    LoopNoTeleport,
    LoopTeleport,
    Signals
}

public struct MovingPlatformSettings : IComponentData
{
    public PlatformMoveMode MoveMode;
    public bool SlowDownNearNodes;
    public TimeValue PauseOnNodesDuration;

    // when slowing down, the platform will reach this mininum speed
    public static readonly fix SLOW_DOWN_MINIMUM_SPEED = (fix)0.1f;

    // how fast can the platform slow down (lower value will cause the platform to start slowing down earlier)
    public static readonly fix SLOW_DOWN_FACTOR_INV = (fix)2.4f;
}

public struct MovingPlatformState : IComponentData
{
    public int NextStep;
    public TimeValue RemainingWaitTime;
}

/// <summary>
/// Used whenever a moving platform uses signals
/// </summary>
public struct MovingPlatformSignalPosition : IBufferElementData
{
    public fix2 Position;
    public Entity ConditionalEmitter;
}

public partial class MovingPlatformSystem : SimGameSystemBase
{
    private struct PathAccessor
    {
        private DynamicBuffer<PathPosition> _buffer;
        private bool _yoyo;

        public int Length => _buffer.Length;

        public PathAccessor(DynamicBuffer<PathPosition> buffer, bool yoyo)
        {
            _buffer = buffer;
            _yoyo = yoyo;
        }

        public fix2 this[int index]
        {
            get
            {
                int remainder = mathX.mod(index, _buffer.Length);

                if (_yoyo && mathX.odd(index / _buffer.Length))
                {
                    remainder = _buffer.Length - 1 - remainder;
                }

                return _buffer[remainder].Position;
            }
        }

        public bool IsStart(int index) => index % _buffer.Length == 0;
    }

    protected override void OnUpdate()
    {
        UpdateSignalPlatforms();
        UpdatePlatformVelocities();
    }

    private void UpdateSignalPlatforms()
    {
        // For each platform with signal positions, remove positions that we have visited
        Entities.WithAll<MovingPlatformSignalPosition>()
            .ForEach((DynamicBuffer<PathPosition> pathPositions, ref MovingPlatformState state) =>
        {
            if (state.NextStep > 0)
            {
                if (pathPositions.Length > 0)
                    pathPositions.RemoveRange(0, min(state.NextStep, pathPositions.Length));
                state.NextStep = 0;
            }
        }).Run();

        // For each platform, find the first position with the signal ON. Append it as the next destination (like a basic elevator)
        Entities.ForEach((DynamicBuffer<PathPosition> pathPositions, DynamicBuffer<MovingPlatformSignalPosition> signalPositions) =>
        {
            fix2 destination = default;
            bool hasDestination = false;
            for (int i = 0; i < signalPositions.Length; i++)
            {
                if (signalPositions[i].ConditionalEmitter == Entity.Null 
                    || (HasComponent<Signal>(signalPositions[i].ConditionalEmitter) && GetComponent<Signal>(signalPositions[i].ConditionalEmitter)))
                {
                    hasDestination = true;
                    destination = signalPositions[i].Position;
                    break;
                }
            }

            if (hasDestination && (pathPositions.Length == 0 || pathPositions[pathPositions.Length - 1] != destination))
            {
                pathPositions.Add(destination);
            }
        }).Run();
    }

    private void UpdatePlatformVelocities()
    {
        var deltaTime = GetDeltaTime();

        // move along path
        Entities.ForEach(
            (Entity entity,
            DynamicBuffer<PathPosition> pathPositions,
            ref PhysicsVelocity velocity,
            ref FixTranslation translation,
            ref MovingPlatformState state,
            in MoveSpeed moveSpeed,
            in MovingPlatformSettings settings) =>
            {
                if (pathPositions.Length == 0)
                {
                    velocity.Linear = fix2.zero;
                    return;
                }

                if (HasComponent<Signal>(entity))
                {
                    Signal signal = GetComponent<Signal>(entity);
                    if (!signal.Value)
                    {
                        velocity.Linear = fix2.zero;
                        return;
                    }
                }

                if (state.RemainingWaitTime > TimeValue.Zero)
                {
                    velocity.Linear = fix2.zero;
                    state.RemainingWaitTime -= deltaTime.GetValue(state.RemainingWaitTime.Type);
                    return;
                }

                var pathAccessor = new PathAccessor(pathPositions, settings.MoveMode == PlatformMoveMode.Yoyo);

                fix actualMoveSpeed = moveSpeed.Value;

                if (settings.SlowDownNearNodes)
                {
                    // If the platform is near the past or next node, clamp speed to smaller value
                    fix nodeDist = sqrt(sqrt(min(
                        lengthsq(translation.Value - pathAccessor[state.NextStep - 1]),
                        lengthsq(translation.Value - pathAccessor[state.NextStep]))));

                    fix maximumSpeed = MovingPlatformSettings.SLOW_DOWN_MINIMUM_SPEED + (nodeDist * MovingPlatformSettings.SLOW_DOWN_FACTOR_INV);

                    actualMoveSpeed = min(maximumSpeed, moveSpeed.Value);
                }

                fix moveDist = actualMoveSpeed * deltaTime.Seconds.Value;

                fix2 a = translation.Value;
                fix2 b = translation.Value;
                fix2 v = fix2(0);
                fix vLength = 0;

                bool incrementStep = false;
                int beginIndex = state.NextStep % pathAccessor.Length;

                while (moveDist > vLength)
                {
                    moveDist -= vLength;
                    a = b;

                    if (incrementStep)
                    {
                        state.NextStep++;

                        if (beginIndex == state.NextStep % pathAccessor.Length) // if we've done a full loop, stop here. This prevents infinite while loops.
                        {
                            break;
                        }

                        if (settings.MoveMode == PlatformMoveMode.LoopTeleport && pathAccessor.IsStart(state.NextStep))
                        {
                            a = pathAccessor[state.NextStep];
                            translation.Value = pathAccessor[state.NextStep];
                            state.NextStep++;
                        }

                        if (settings.PauseOnNodesDuration > TimeValue.Zero)
                        {
                            state.RemainingWaitTime = settings.PauseOnNodesDuration;
                        }
                    }

                    incrementStep = true;
                    b = pathAccessor[state.NextStep];

                    v = b - a;
                    vLength = length(v);
                }

                fix2 dir = (vLength == 0 ? fix2(0) : (v / vLength));
                fix2 targetPosition = a + (dir * min(moveDist, vLength));
                fix2 targetVelocity = (targetPosition - translation.Value) / deltaTime.Seconds.Value;
                velocity.Linear = targetVelocity;
            }).Run();
    }
}