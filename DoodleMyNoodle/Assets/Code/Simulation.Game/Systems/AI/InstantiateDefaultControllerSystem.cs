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
    private NativeList<Entity> _toSpawnPawns;
    private NativeList<Entity> _toSpawnControllerPrefabs;

    protected override void OnCreate()
    {
        base.OnCreate();
        _toSpawnPawns = new NativeList<Entity>(Allocator.Persistent);
        _toSpawnControllerPrefabs = new NativeList<Entity>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _toSpawnPawns.Dispose();
        _toSpawnControllerPrefabs.Dispose();
    }

    protected override void OnUpdate()
    {
        var controllers = GetComponentDataFromEntity<ControlledEntity>(isReadOnly: true);

        var toSpawnPawns = _toSpawnPawns;
        var toSpawnControllerPrefabs = _toSpawnControllerPrefabs;

        // Find which pawn needs a controller
        Entities
            .ForEach((Entity pawn, in Controllable controllable, in DefaultControllerPrefab defaultControllerPrefab, in Health health) =>
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

                if (controllers.HasComponent(controllable.CurrentController))
                {
                    // current controller exits
                    return;
                }

                toSpawnPawns.Add(pawn);
                toSpawnControllerPrefabs.Add(defaultControllerPrefab.Value);

            }).Run();

        // instantiate controllers
        if (toSpawnControllerPrefabs.Length > 0)
        {
            var spawnedControllers = new NativeArray<Entity>(toSpawnControllerPrefabs.Length, Allocator.Temp);
            EntityManager.Instantiate(toSpawnControllerPrefabs, spawnedControllers);

            // bind controllers and pawns
            for (int i = 0; i < spawnedControllers.Length; i++)
            {
                Entity newController = spawnedControllers[i];
                Entity pawn = toSpawnPawns[i];

                EntityManager.SetComponentData(newController, new ControlledEntity() { Value = pawn });
                EntityManager.SetComponentData(pawn, new Controllable() { CurrentController = newController });
            }

            toSpawnPawns.Clear();
            toSpawnControllerPrefabs.Clear();
        }
    }
}