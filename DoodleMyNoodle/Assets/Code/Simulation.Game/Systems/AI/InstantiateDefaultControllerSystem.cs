using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

/// <summary>
/// This system instantiates controllable entities' DefaultControllerPrefab when needed
/// </summary>
public class InstantiateDefaultControllerSystem : SimSystemBase
{
    private struct SpawnRequest
    {
        public Entity Pawn;
        public Entity ControllerPrefab;
        public int Team;
    }

    private NativeList<SpawnRequest> _spawnRequests;

    protected override void OnCreate()
    {
        base.OnCreate();
        _spawnRequests = new NativeList<SpawnRequest>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _spawnRequests.Dispose();
    }

    protected override void OnUpdate()
    {
        var spawnRequests = _spawnRequests;

        // Find which pawn needs a controller
        Entities
            .ForEach((Entity pawn, in Controllable controllable, in DefaultControllerPrefab defaultControllerPrefab, in DefaultControllerTeam defaultTeam, in Health health) =>
            {
                if (defaultControllerPrefab.Value == Entity.Null)
                {
                    // default prefab is null
                    return;
                }

                if (health.Value <= 0)
                {
                    // pawn dead
                    return;
                }

                if (HasComponent<ControlledEntity>(controllable.CurrentController))
                {
                    // current controller exits
                    return;
                }

                spawnRequests.Add(new SpawnRequest()
                {
                    Pawn = pawn,
                    ControllerPrefab = defaultControllerPrefab.Value,
                    Team = defaultTeam.Value,
                });
            }).Run();

        // instantiate controllers
        if (spawnRequests.Length > 0)
        {
            for (int i = 0; i < spawnRequests.Length; i++)
            {
                var spawnRequest = spawnRequests[i];

                // bind controllers and pawns
                Entity newController = EntityManager.Instantiate(spawnRequest.ControllerPrefab);

                SetComponent(newController, new Team() { Value = spawnRequest.Team });
                SetComponent(newController, new ControlledEntity() { Value = spawnRequest.Pawn });
                SetComponent(spawnRequest.Pawn, new Controllable() { CurrentController = newController });
            }

            spawnRequests.Clear();
        }
    }
}