using CCC.Fix2D;
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
    LoopTeleport
}

public struct MovingPlatformSettings : IComponentData
{
    public PlatformMoveMode MoveMode;
    public bool SlowDownNearNodes;
    public fix PauseOnNodesDuration;

    // when slowing down, the platform will reach this mininum speed
    public static readonly fix SLOW_DOWN_MINIMUM_SPEED = (fix)0.1f;
    
    // how fast can the platform slow down (lower value will cause the platform to start slowing down earlier)
    public static readonly fix SLOW_DOWN_FACTOR_INV = (fix)2.4f;
}

public struct MovingPlatformState : IComponentData
{
    public int NextStep;
    public fix RemainingWaitTime;
}

public class MovingPlatformSystem : SimSystemBase
{
    private struct PathAccessor
    {
        private DynamicBuffer<PathPosition> _buffer;
        private bool _yoyo;

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
        var deltaTime = Time.DeltaTime;

        // move along path
        Entities.ForEach(
            (DynamicBuffer<PathPosition> pathPositions,
            ref PhysicsVelocity velocity,
            ref FixTranslation translation,
            ref MovingPlatformState state,
            in MoveSpeed moveSpeed,
            in MovingPlatformSettings settings) =>
            {
                if (pathPositions.Length < 2)
                {
                    return;
                }

                if(state.RemainingWaitTime > 0)
                {
                    velocity.Linear = fix2.zero;
                    state.RemainingWaitTime -= deltaTime;
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

                fix moveDist = actualMoveSpeed * deltaTime;

                fix2 a = translation.Value;
                fix2 b = translation.Value;
                fix2 v = fix2(0);
                fix vLength = 0;

                bool incrementStep = false;

                while (moveDist > vLength)
                {
                    moveDist -= vLength;
                    a = b;

                    if (incrementStep)
                    {
                        state.NextStep++;

                        if (settings.MoveMode == PlatformMoveMode.LoopTeleport && pathAccessor.IsStart(state.NextStep))
                        {
                            a = pathAccessor[state.NextStep];
                            translation.Value = pathAccessor[state.NextStep];
                            state.NextStep++;
                        }

                        if(settings.PauseOnNodesDuration > 0)
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
                fix2 targetVelocity = (targetPosition - translation.Value) / deltaTime;
                velocity.Linear = targetVelocity;
            }).Run();
    }
}