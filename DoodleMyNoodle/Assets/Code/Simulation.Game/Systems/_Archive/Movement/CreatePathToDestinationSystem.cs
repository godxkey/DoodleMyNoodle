using Unity.Mathematics;
using Unity.Entities;
using static fixMath;
using Unity.Collections;
using CCC.Fix2D;

[UpdateInGroup(typeof(MovementSystemGroup))]
public class CreatePathToDestinationSystem : SimSystemBase
{
    private PhysicsWorldSystem _physicsWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<GridInfo>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnUpdate()
    {
        var tileWorld = CommonReads.GetTileWorld(Accessor);
        var pathResult = new Pathfinding.PathResult(Allocator.Temp);
        var physicsWorld = _physicsWorldSystem.PhysicsWorldSafe;
        var entity2PhysicsBody = _physicsWorldSystem.EntityToPhysicsBody;

        Entities
            .WithReadOnly(tileWorld)
            .ForEach((Entity entity, in FixTranslation pos, in Destination destination, in MoveSpeed moveSpeed) =>
        {
            int2 from = Helpers.GetTile(pos);
            int2 to = Helpers.GetTile(destination.Value);

            Entity jumpItem = CommonReads.FindFirstItemWithGameAction<GameActionBasicJump>(Accessor, entity);

            if (!entity2PhysicsBody.Lookup.TryGetValue(entity, out int physicsBodyIndex))
                physicsBodyIndex = -1;
            
            var pathfindingContext = new Pathfinding.Context(tileWorld, physicsWorld, physicsBodyIndex, Pathfinding.AgentCapabilities.DefaultMaxCost);
            pathfindingContext.AgentCapabilities = new Pathfinding.AgentCapabilities()
            {
                Drop1TileCost = 0,
                Jump1TileCost = HasComponent<GameActionSettingAPCost>(jumpItem) ? GetComponent<GameActionSettingAPCost>(jumpItem).Value : 0,
                Walk1TileCost = moveSpeed.Value <= fix.Zero ? fix.MaxValue : fix.One / moveSpeed.Value,
            };

            bool pathFound = Pathfinding.FindNavigablePath(
                context: pathfindingContext,
                startPos: pos,
                goalPos: destination,
                reachDistance: 0,
                result: ref pathResult);

            if (pathFound)
            {
                DynamicBuffer<PathPosition> pathBuffer = EntityManager.GetOrAddBuffer<PathPosition>(entity);
                pathBuffer.Clear();

                for (int i = 1; i < pathResult.Segments.Length - 1; i++) // exclude last point since we add 'destination' last
                {
                    pathBuffer.Add(new PathPosition() { Position = pathResult.Segments[i].EndPosition });
                }

                pathBuffer.Add(destination.Value);
            }

            EntityManager.RemoveComponent<Destination>(entity);
        })
            .WithStructuralChanges()
            .WithoutBurst()
            .Run();
    }
}