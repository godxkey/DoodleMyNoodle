using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;

/// <summary>
/// The horizontal position between the entity and its attack target
/// </summary>
public struct OffsetFromTarget : IComponentData
{
    public fix Value;

    public fix Distance => abs(Value);

    public static implicit operator fix(OffsetFromTarget val) => val.Value;
    public static implicit operator OffsetFromTarget(fix val) => new OffsetFromTarget() { Value = val };
}

[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateBefore(typeof(UpdateMovementSystem))]
public class UpdateTargetRelativePositionSystem : SimGameSystemBase
{
    private PhysicsWorldSystem _physicsWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
    }

    protected override void OnUpdate()
    {
        // no players ? consider great distance
        if (!HasSingleton<PlayerGroupDataTag>())
        {
            Entities
                .ForEach((ref OffsetFromTarget offset) =>
                {
                    offset.Value = 1000;
                }).Run();
            return;
        }


        Entity playerGroupEntity = GetSingletonEntity<PlayerGroupDataTag>();
        fix2 playerGroupPosition = GetComponent<FixTranslation>(playerGroupEntity);
        fix playerGroupFront = playerGroupPosition.x + SimulationGameConstants.CharacterRadius;

        Entities
            .ForEach((ref OffsetFromTarget offset, in FixTranslation position) =>
            {
                offset.Value = position.Value.x - playerGroupFront;
            }).Run();


        // Set the target-relative distance of the player group.
        // Since there is no 'header' for enemies, we use a sort of hack using the 'StopMoveFromTargetDistance'.
        // We make an overlap test, and if there are any contacts, we say the player is close enough.
        // 
        // If we add allied mobs, we'll need to code something better than this. We can probably just find the left-most enemy using a few raycasts.
        // We just have to think about what this means for flying mobs, do we ignore them?
        {
            fix2 triggerSize = fix2(GetComponent<DesiredRangeFromTarget>(playerGroupEntity).Value.Max, (fix)0.5);
            fix2 min = playerGroupPosition - new fix2(0, triggerSize.y / 2);
            fix2 max = min + triggerSize;

            min.x += SimulationGameConstants.CharacterRadius + (fix)0.05; // to avoid hitting the front player

            OverlapAabbInput overlapAabbInput = new OverlapAabbInput()
            {
                Aabb = new Aabb((float2)min, (float2)max),
                Filter = SimulationGameConstants.Physics.CollideWithCharactersFilter.Data,
            };
            NativeList<int> touchedbodies = new NativeList<int>(Allocator.Temp);
            PhysicsWorld physicsWorld = _physicsWorldSystem.PhysicsWorld;

            // a dumb way to stop the player. Maybe revisit this later if we need actual distance
            fix offsetFromTarget = (fix)(physicsWorld.OverlapAabb(overlapAabbInput, touchedbodies) ? 0f : -99f);
            SetComponent<OffsetFromTarget>(playerGroupEntity, offsetFromTarget);
        }
    }
}