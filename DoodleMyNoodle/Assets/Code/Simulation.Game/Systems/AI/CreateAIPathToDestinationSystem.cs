using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

[UpdateInGroup(typeof(AISystemGroup))]
public class CreateAIPathToDestinationSystem : SimSystemBase
{
    private NativeList<int2> _tilePath;
    private UpdateActorWorldSystem _actorWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _tilePath = new NativeList<int2>(Allocator.Persistent);
        _actorWorldSystem = World.GetOrCreateSystem<UpdateActorWorldSystem>();
        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _tilePath.Dispose();
    }

    protected override void OnUpdate()
    {
        fix time = Time.ElapsedTime;
        fix repathCooldown = SimulationGameConstants.AgentRepathCooldown;
        TileWorld tileWorld = CommonReads.GetTileWorld(Accessor);
        NativeList<int2> pathfindingResult = _tilePath;
        ActorWorld actorWorld = _actorWorldSystem.ActorWorld;

        Entities
            .WithReadOnly(tileWorld)
            .ForEach((Entity entity, DynamicBuffer<AIPathPosition> pathBuffer, ref AIDestinationRepathData repathData, in AIDestination destination, in ControlledEntity pawn) =>
        {
            if (destination.HasDestination)
            {
                // only recalculate path if older than Xs
                if (repathData.PathCreatedPosition == destination.Position
                    && repathData.PathCreatedTime + repathCooldown >= time)
                    return;

                pathBuffer.Clear();

                int pawnIndex = actorWorld.GetPawnIndex(pawn);
                if (pawnIndex == -1)
                    return;

                ref var pawnData = ref actorWorld.GetPawn(pawnIndex);

                int2 from = Helpers.GetTile(pawnData.Position);
                int2 to = Helpers.GetTile(destination.Position);

                bool pathFound = Pathfinding.FindNavigablePath(tileWorld, from, to, Pathfinding.MAX_PATH_LENGTH, pathfindingResult);

                if (pathFound)
                {
                    fix2 feetOffset = fix2(0, pawnData.Radius);

                    for (int i = 1; i < pathfindingResult.Length - 1; i++) // exclude last point since we add 'destination' last
                    {
                        pathBuffer.Add(Helpers.GetTileBottom(pathfindingResult[i]) + feetOffset);
                    }

                    fix2 dest = destination.Position;

                    if (!tileWorld.GetFlags(Helpers.GetTile(dest)).IsLadder) // lower destination to ground
                        dest.y = Helpers.GetTileBottom(destination.Position).y + feetOffset.y;

                    pathBuffer.Add(dest);
                }

                repathData.PathCreatedPosition = destination.Position;
                repathData.PathCreatedTime = time;
            }
            else
            {
                if (!pathBuffer.IsEmpty)
                    pathBuffer.Clear();
                repathData.PathCreatedTime = -1;
            }
        }).Schedule();
    }
}
