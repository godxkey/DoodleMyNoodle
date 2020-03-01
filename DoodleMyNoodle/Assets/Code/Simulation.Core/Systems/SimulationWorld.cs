using System.Diagnostics;
using Unity.Entities;

[DebuggerDisplay("{Name} (#{SequenceNumber})")]
public class SimulationWorld : World
{
    public static SimulationWorld Instance { get; set; }

    public SimInput[] OngoingTickInputs;

    public SimulationWorld(string name) : base(name)
    {
    }
}
