using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

public struct Seed : IComponentData
{
    public Seed(uint value) { Value = value; }

    public uint Value;
}

[AlwaysUpdateSystem]
public class InitializeRandomSeedSystem : ComponentSystem
{
    protected Entity SeedSingleton
    {
        get
        {
            if (_seedSingletonQuery.IsEmptyIgnoreFilter)
            {
                var entity = EntityManager.CreateEntity(typeof(Seed));
#if UNITY_EDITOR
                EntityManager.SetName(entity, "WorldSeed");
#endif
                Seed seed = new Seed(World is SimulationWorld simWorld ? simWorld.SeedToPickIfInitializing : 0);
                EntityManager.SetComponentData(entity, seed);
            }

            return _seedSingletonQuery.GetSingletonEntity();
        }
    }

    private EntityQuery _seedSingletonQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _seedSingletonQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Seed>());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        _seedSingletonQuery.Dispose();
    }

    protected override void OnUpdate()
    {
        if (World is SimulationWorld simWorld)
        {
            Seed seed = EntityManager.GetComponentData<Seed>(SeedSingleton);

            simWorld.Seed = seed.Value;
            simWorld.RandomModule = new WorldModuleTickRandom(simWorld.LatestTickId, seed.Value);
        }
    }
}
