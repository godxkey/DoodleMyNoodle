using System.Diagnostics;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;

public readonly struct FixTimeData
{
    /// <summary>
    /// The total cumulative elapsed time in seconds.
    /// </summary>
    public readonly Fix64 ElapsedTime;

    /// <summary>
    /// The time in seconds since the last time-updating event occurred. (For example, a frame.)
    /// </summary>
    public readonly Fix64 DeltaTime;

    /// <summary>
    /// Create a new TimeData struct with the given values.
    /// </summary>
    /// <param name="elapsedTime">Time since the start of time collection.</param>
    /// <param name="deltaTime">Elapsed time since the last time-updating event occurred.</param>
    public FixTimeData(Fix64 elapsedTime, Fix64 deltaTime)
    {
        ElapsedTime = elapsedTime;
        DeltaTime = deltaTime;
    }
}

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
}