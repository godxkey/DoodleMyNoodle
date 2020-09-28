using System;
using System.Diagnostics;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;

[DebuggerDisplay("{Name} (#{SequenceNumber})")]
public class SimulationWorld : World, IOwnedWorld
{
    // The SimulationWorldSystem instance (in another world) that owns this world
    public IWorldOwner Owner { get; set; }
    public SimulationWorld(string name) : base(name)
    {
        _tickSingletonQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<SimulationOngoingTickId>());
    }

    internal uint SeedToPickIfInitializing;
    public uint ExpectedNewTickId { get; internal set; }
    public SimInput[] TickInputs { get; internal set; }

    public FixRandom Random() => RandomModule.PickRandomGenerator();

    // cached value - the real data is on an entity
    internal FixTimeData CurrentFixTime;
    public ref FixTimeData FixTime => ref CurrentFixTime;
    public new bool Time => throw new System.Exception("Use FixTime instead!");

    // cached value - the real data is on an entity
    public uint LatestTickId { get; internal set; }

    // cached value - the real data is on an entity
    public uint Seed { get; internal set; }

    // cached value - the real data is on an entity
    internal WorldModuleTickRandom RandomModule;

    // provides easy access to data
    private InternalSimWorldAccessor _internalAccessor;
    public InternalSimWorldAccessor GetInternalAccessor()
    {
        if (_internalAccessor == null)
        {
            _internalAccessor = new InternalSimWorldAccessor();
            _internalAccessor.SimWorld = this;
            _internalAccessor.EntityManager = EntityManager;
            _internalAccessor.SomeSimSystem = GetOrCreateSystem<SimPreInitializationSystemGroup>();
        }

        return _internalAccessor;
    }

    private EntityQuery _tickSingletonQuery;
    internal uint GetLastedTickIdFromEntity()
    {
        if (TryGetTickDataSingleton(out Entity singleton))
        {
            return EntityManager.GetComponentData<SimulationOngoingTickId>(singleton).TickId;
        }
        return 0;
    }

    internal bool TryGetTickDataSingleton(out Entity singleton)
    {
        if (!_tickSingletonQuery.IsEmptyIgnoreFilter)
        {
            singleton = _tickSingletonQuery.GetSingletonEntity();
        }
        else
        {
            singleton = Entity.Null;
        }

        return singleton != Entity.Null;
    }

    internal Entity GetOrCreateTickDataSingleton()
    {
        if (_tickSingletonQuery.IsEmptyIgnoreFilter)
        {
#if UNITY_EDITOR
            var entity = EntityManager.CreateEntity(typeof(SimulationOngoingTickId));
            EntityManager.SetName(entity, "WorldTick");
#else
            EntityManager.CreateEntity(typeof(SimulationOngoingTickId));
#endif
        }

        return _tickSingletonQuery.GetSingletonEntity();
    }
}
