#if UNITY_EDITOR
#define GIZMOS
#endif

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using Unity.Jobs;

[UpdateInGroup(typeof(PreAISystemGroup))]
public class UpdateActorWorldSystem : SimSystemBase
{
    private PhysicsWorldSystem _physicsSystemGroup;
    private EntityQuery _pawnsGroup;
    public ActorWorld ActorWorld;
    public JobHandle ActorWorldDependency;

    protected override void OnCreate()
    {
        base.OnCreate();

        _physicsSystemGroup = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _pawnsGroup = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Health>(),
            ComponentType.ReadOnly<Controllable>(),
            ComponentType.ReadOnly<FixTranslation>());

        ActorWorld = new ActorWorld(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ActorWorld.Dispose();
    }

    protected override void OnUpdate()
    {
        var pawns = _pawnsGroup.ToEntityArrayAsync(Allocator.TempJob, out JobHandle getPawnsJobHandle);
        var controllers = _pawnsGroup.ToComponentDataArrayAsync<Controllable>(Allocator.TempJob, out JobHandle getControllersJob);
        var positions = _pawnsGroup.ToComponentDataArrayAsync<FixTranslation>(Allocator.TempJob, out JobHandle getPositionsJob);
        var entity2BodyIndex = _physicsSystemGroup.EntityToPhysicsBody.Lookup;

        Dependency = JobHandle.CombineDependencies(new NativeList<JobHandle>(Allocator.Temp) { Dependency, getPawnsJobHandle, getControllersJob, getPositionsJob });

        var lookup = ActorWorld;
        Job
            .WithDisposeOnCompletion(pawns)
            .WithDisposeOnCompletion(controllers)
            .WithDisposeOnCompletion(positions)
            .WithCode(() =>
        {
            // clear
            for (int t = 0; t < ActorWorld.TEAM_COUNT; t++)
            {
                lookup.GetTeamPawns(t).Clear();
            }
            lookup.Pawns.Clear();
            lookup.Entity2PawnIndex.Clear();

            for (int i = 0; i < pawns.Length; i++)
            {
                var controller = controllers[i].CurrentController;
                var pawn = pawns[i];
                bool controllerValid = HasComponent<ControlledEntity>(controller) && GetComponent<ControlledEntity>(controller) == pawns[i];
                fix radius = fix(0.5f);
                if (HasComponent<PhysicsColliderBlob>(pawn))
                {
                    PhysicsColliderBlob collider = GetComponent<PhysicsColliderBlob>(pawn);
                    if (collider.Collider.IsCreated)
                    {
                        radius = (fix)GetComponent<PhysicsColliderBlob>(pawn).Collider.Value.Radius;
                    }
                }

                // Create pawn element
                ActorWorld.Pawn pawnElement = new ActorWorld.Pawn()
                {
                    Entity = pawn,
                    Team = controllerValid ? GetComponent<Team>(controller) : Team.None,
                    Dead = HasComponent<DeadTag>(pawn),
                    Position = positions[i],
                    Tile = Helpers.GetTile(positions[i]),
                    Radius = radius,
                    BodyIndex = entity2BodyIndex.TryGetValue(pawn, out int b) ? b : -1,
                };

                int pawnIndex = lookup.Pawns.Length;

                // Add to lookups
                lookup.Pawns.Add(pawnElement);
                lookup.Entity2PawnIndex.Add(pawnElement.Entity, pawnIndex);
                lookup.GetTeamPawns(ActorWorld.GetTeamIndex(pawnElement.Team)).Add(pawnIndex);
            }

        }).Schedule();

        ActorWorldDependency = Dependency;
    }
}


public struct ActorWorld : IDisposable
{
    public const int INVALID_TEAM_INDEX = 3;
    public const int TEAM_COUNT = 4;

    public struct Pawn
    {
        public Entity Entity;
        public Team Team;
        public fix2 Position;
        public int2 Tile;
        public bool Dead;
        public fix Radius;
        public int BodyIndex;
    }

    public struct PawnSightQueryInput
    {
        public TileWorld TileWorld;
        public Team? ExcludeTeam;
        public bool ExcludeDead;
        public fix2 EyeLocation;
        public fix SightRange;
    }

    public struct PawnSightQueryData
    {
        public fix SightRangeSquared;
    }

    public NativeList<int> TeamsPawns0;
    public NativeList<int> TeamsPawns1;
    public NativeList<int> TeamsPawns2;
    public NativeList<int> TeamsPawnsNone;
    public NativeList<Pawn> Pawns;
    public NativeHashMap<Entity, int> Entity2PawnIndex;

    public ActorWorld(Allocator allocator)
    {
        Pawns = new NativeList<Pawn>(allocator);
        TeamsPawns0 = new NativeList<int>(allocator);
        TeamsPawns1 = new NativeList<int>(allocator);
        TeamsPawns2 = new NativeList<int>(allocator);
        TeamsPawnsNone = new NativeList<int>(allocator);
        Entity2PawnIndex = new NativeHashMap<Entity, int>(64, allocator);
    }

    public void Dispose()
    {
        TeamsPawns0.Dispose();
        TeamsPawns1.Dispose();
        TeamsPawns2.Dispose();
        TeamsPawnsNone.Dispose();
        Pawns.Dispose();
        Entity2PawnIndex.Dispose();
    }

    public ref Pawn GetPawn(int pawnIndex) => ref Pawns.ElementAt(pawnIndex);

    public int GetPawnIndex(Entity pawn)
    {
        return Entity2PawnIndex.TryGetValue(pawn, out int index) ? index : -1;
    }

    public NativeList<int> GetTeamPawns(int teamIndex)
    {
        switch (teamIndex)
        {
            case 0:
                return TeamsPawns0;

            case 1:
                return TeamsPawns1;

            case 2:
                return TeamsPawns2;

            case 3:
                return TeamsPawnsNone;

            default:
                throw new System.NotImplementedException();
        }
    }

    public static int GetTeamIndex(Team team) => team.Value < 0 || team.Value >= INVALID_TEAM_INDEX ? INVALID_TEAM_INDEX : team.Value;

    public void FindAllPawnsInSight(PawnSightQueryInput input, NativeList<int> result)
    {
        var queryData = new PawnSightQueryData();
        queryData.SightRangeSquared = input.SightRange * input.SightRange;

        if (input.ExcludeTeam != null)
        {
            Team excludedTeam = input.ExcludeTeam.Value;
            int excludeCount = GetTeamPawns(excludedTeam).Length;

            if (excludeCount > Pawns.Length / 2)
            {
                // Loop on teams, then pawns
                int excludeTeamIndex = GetTeamIndex(excludedTeam);
                for (int t = 0; t < TEAM_COUNT; t++)
                {
                    if (t == excludeTeamIndex)
                        continue;

                    var teamPawns = GetTeamPawns(t);

                    for (int p = 0; p < teamPawns.Length; p++)
                    {
                        Internal_FindAllPawnsInSight_Element(ref input, ref queryData, teamPawns[p], result);
                    }
                }
            }
            else
            {
                // Loop on pawns, exclude some based on team
                for (int p = 0; p < Pawns.Length; p++)
                {
                    if (Pawns[p].Team != excludedTeam)
                        Internal_FindAllPawnsInSight_Element(ref input, ref queryData, p, result);
                }
            }
        }
        else
        {
            // Loop on all pawns
            for (int p = 0; p < Pawns.Length; p++)
            {
                Internal_FindAllPawnsInSight_Element(ref input, ref queryData, p, result);
            }
        }
    }

    private void Internal_FindAllPawnsInSight_Element(ref PawnSightQueryInput input, ref PawnSightQueryData data, int pawnIndex, NativeList<int> result)
    {
        Pawn pawn = Pawns[pawnIndex];
        if (input.ExcludeDead && pawn.Dead)
            return;

        if (distancesq(input.EyeLocation, pawn.Position) > data.SightRangeSquared)
            return;

        fix sampleVerticalRange  = pawn.Radius * fix(0.5);
        fix2 pawnFeet = pawn.Position - fix2(0, sampleVerticalRange);
        fix2 pawnHead = pawn.Position + fix2(0, sampleVerticalRange);

        if (TilePhysics.RaycastTerrain(input.TileWorld, input.EyeLocation, pawnHead)
            && TilePhysics.RaycastTerrain(input.TileWorld, input.EyeLocation, pawnFeet))
            return;

        result.Add(pawnIndex);
    }
}