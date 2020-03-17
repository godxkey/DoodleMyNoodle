using System.Diagnostics;
using Unity.Entities;

[DebuggerDisplay("{Name} (#{SequenceNumber})")]
public class SimulationWorld : World, IOwnedWorld
{
    public uint ExpectedNewTickId;
    public SimInput[] OngoingTickInputs;

    // TODO fbessette: move this out of here. The simulation shouldn't know
    public uint EntityClearAndReplaceCount = 0;
    
    private EntityQuery _tickSingletonQuery;


    public uint LatestTickId => EntityManager.GetComponentData<SimulationOngoingTickId>(TickDataSingleton).TickId;

    internal Entity TickDataSingleton
    {
        get
        {
            if (_tickSingletonQuery.IsEmptyIgnoreFilter)
            {
#if UNITY_EDITOR
                var entity = EntityManager.CreateEntity(typeof(SimulationOngoingTickId));
                EntityManager.SetName(entity, "World Tick");
#else
                EntityManager.CreateEntity(typeof(SimulationOngoingTickId));
#endif
            }

            return _tickSingletonQuery.GetSingletonEntity();
        }
    }

    // The SimulationWorldSystem instance (in another world) that owns this world
    public IWorldOwner Owner { get; set; }

    public SimulationWorld(string name) : base(name)
    {
        _tickSingletonQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<SimulationOngoingTickId>());
    }
}