using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulationBase
{
    /// <summary>
    /// Is the simulation ready to run ?
    /// </summary>
    public static bool IsInitialized => SimModules.World != null;

    /// <summary>
    /// Can the simulation be ticked ? Some things can prevent the simulation to be ticked (like being in the middle of a scene injection)
    /// </summary>
    public static bool CanBeTicked => SimModules.Ticker.CanSimBeTicked;

    /// <summary>
    /// Can the simulation be saved ? Some things can prevent the simulation to be saved (like being in the middle of a scene injection)
    /// </summary>
    public static bool CanBeSaved => SimModules.Serializer.CanSimWorldBeSaved;

    /// <summary>
    /// The simulation tick delta time. Should be constant for the simulation to stay deterministic
    /// </summary>
    public static readonly Fix64 DeltaTime = SimulationConstants.TIME_STEP; // 50 ticks per seconds

    /// <summary>
    /// The current simulation tick id. Increments by 1 every Tick()
    /// </summary>
    public static uint TickId => SimModules.World.TickId;
    public static void Tick(in SimTickData tickData) => SimModules.Ticker.Tick(tickData);

    public static SimBlueprint GetBlueprint(in SimBlueprintId blueprintId) => SimModules.BlueprintBank.GetBlueprint(blueprintId);

    public static SimEntity FindEntityWithName(string name) => SimModules.WorldSearcher.FindEntityWithName(name);
    public static SimEntity FindEntityWithComponent<T>() => SimModules.WorldSearcher.FindEntityWithComponent<T>();
    public static SimEntity FindEntityWithComponent<T>(out T comp) => SimModules.WorldSearcher.FindEntityWithComponent(out comp);

    public static ReadOnlyList<SimEntity>          Entities                                 => new ReadOnlyList<SimEntity>(SimModules.World.Entities);
    public static EntityEnumerable<T1>             EntitiesWithComponent<T1>()              => new EntityEnumerable<T1>(SimModules.World.Entities);
    public static EntityEnumerable<T1, T2>         EntitiesWithComponents<T1, T2>()         => new EntityEnumerable<T1, T2>(SimModules.World.Entities);
    public static EntityEnumerable<T1, T2, T3>     EntitiesWithComponents<T1, T2, T3>()     => new EntityEnumerable<T1, T2, T3>(SimModules.World.Entities);
    public static EntityEnumerable<T1, T2, T3, T4> EntitiesWithComponents<T1, T2, T3, T4>() => new EntityEnumerable<T1, T2, T3, T4>(SimModules.World.Entities);
}
