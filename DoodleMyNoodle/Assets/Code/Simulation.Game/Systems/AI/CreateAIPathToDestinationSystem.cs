using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

[UpdateInGroup(typeof(AISystemGroup))]
public class CreateAIPathToDestinationSystem : SimSystemBase
{
    private UpdateActorWorldSystem _actorWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _actorWorldSystem = World.GetOrCreateSystem<UpdateActorWorldSystem>();
        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnUpdate()
    {
        fix time = Time.ElapsedTime;
        fix repathCooldown = SimulationGameConstants.AgentRepathCooldown;
        TileWorld tileWorld = CommonReads.GetTileWorld(Accessor);
        Pathfinding.PathResult pathResult = new Pathfinding.PathResult(Allocator.TempJob);
        ActorWorld actorWorld = _actorWorldSystem.ActorWorld;

        Entities
            .WithDisposeOnCompletion(pathResult)
            .WithReadOnly(tileWorld)
            .ForEach((Entity entity, DynamicBuffer<AIPathSegment> pathBuffer, ref AIDestinationRepathData repathData, in AIDestination destination, in ControlledEntity pawn) =>
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

                var moveSpeed = GetComponent<MoveSpeed>(pawn);
                Entity jumpItem = Entity.Null;
                {
                    var pawnInventory = GetBuffer<InventoryItemReference>(pawn);
                    var meleeAttackActionId = GameActionBank.GetActionId<GameActionBasicJump>();
                    Entity meleeAttackItem = Entity.Null;
                    for (int i = 0; i < pawnInventory.Length; i++)
                    {
                        if (GetComponent<GameActionId>(pawnInventory[i].ItemEntity) == meleeAttackActionId)
                        {
                            jumpItem = pawnInventory[i].ItemEntity;
                            break;
                        }
                    }
                }
                var pathfindingContext = new Pathfinding.Context(tileWorld);
                pathfindingContext.AgentCapabilities = new Pathfinding.AgentCapabilities()
                {
                    Drop1TileCost = 0,
                    Jump1TileCost = HasComponent<GameActionSettingAPCost>(jumpItem) ? GetComponent<GameActionSettingAPCost>(jumpItem).Value : 0,
                    Walk1TileCost = moveSpeed.Value <= fix.Zero ? fix.MaxValue : fix.One / moveSpeed.Value,
                };

                bool pathFound = Pathfinding.FindNavigablePath(pathfindingContext, pawnData.Position, destination.Position, Pathfinding.AgentCapabilities.DefaultMaxCost, ref pathResult);

                if (pathFound)
                {
                    fix2 feetOffset = fix2(0, pawnData.Radius);

                    for (int i = 0; i < pathResult.Segments.Length - 1; i++) // exclude last point since we add 'destination' last
                    {
                        Pathfinding.Segment pathSegment = pathResult.Segments[i];
                        pathSegment.EndPosition = Helpers.GetTileBottom(pathSegment.EndTile) + feetOffset;
                        pathBuffer.Add(pathSegment);
                    }

                    var lastSegment = pathResult.Segments.Last();
                    lastSegment.EndPosition = destination.Position;

                    if (!tileWorld.GetFlags(lastSegment.EndTile).IsLadder) // lower destination to ground
                        lastSegment.EndPosition.y = Helpers.GetTileBottom(lastSegment.EndTile).y + feetOffset.y;

                    pathBuffer.Add(lastSegment);
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
