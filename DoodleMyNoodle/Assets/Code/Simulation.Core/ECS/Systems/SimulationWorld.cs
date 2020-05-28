using System.Diagnostics;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;

[DebuggerDisplay("{Name} (#{SequenceNumber})")]
public class SimulationWorld : World, IOwnedWorld
{
    // The SimulationWorldSystem instance (in another world) that owns this world
    public IWorldOwner Owner { get; set; }
    public SimulationWorld(string name) : base(name)
    {
        _tickSingletonQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<SimulationOngoingTickId>());
    }


    // TODO fbessette: move this out of here. The simulation shouldn't know
    public uint EntityClearAndReplaceCount { get; internal set; } = 0;



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
            UnityEngine.Debug.Log("Create internal accessor");
        }

        return _internalAccessor;
    }

    private EntityQuery _tickSingletonQuery;
    internal uint GetLastedTickIdFromEntity() { return EntityManager.GetComponentData<SimulationOngoingTickId>(TickDataSingleton).TickId; }
    internal Entity TickDataSingleton
    {
        get
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
}
