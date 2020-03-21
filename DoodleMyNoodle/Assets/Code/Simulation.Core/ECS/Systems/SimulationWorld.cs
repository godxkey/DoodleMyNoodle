using System.Diagnostics;
using Unity.Entities;
using Unity.Mathematics;

[DebuggerDisplay("{Name} (#{SequenceNumber})")]
public class SimulationWorld : World, IOwnedWorld
{
    // The SimulationWorldSystem instance (in another world) that owns this world
    public IWorldOwner Owner { get; set; }
    public SimulationWorld(string name) : base(name) { }


    // TODO fbessette: move this out of here. The simulation shouldn't know
    public uint EntityClearAndReplaceCount { get; internal set; } = 0;


    internal uint SeedToPickIfInitializing;
    public uint ExpectedNewTickId { get; internal set; }
    public SimInput[] OngoingTickInputs { get; internal set; }

    public Random Random() => RandomModule.PickRandomGenerator();

    // cached value - the real data is on an entity
    public uint LatestTickId { get; internal set; }
    
    // cached value - the real data is on an entity
    public uint Seed { get; internal set; }

    // cached value - the real data is on an entity
    internal WorldModuleTickRandom RandomModule;
}