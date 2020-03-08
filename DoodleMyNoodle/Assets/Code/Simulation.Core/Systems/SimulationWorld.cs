using System.Diagnostics;
using Unity.Entities;

[DebuggerDisplay("{Name} (#{SequenceNumber})")]
public class SimulationWorld : World, IOwnedWorld
{
    public SimInput[] OngoingTickInputs;

    // The SimulationWorldSystem instance (in another world) that owns this world
    public IWorldOwner Owner { get; set; }

    public SimulationWorld(string name) : base(name)
    {
    }
}