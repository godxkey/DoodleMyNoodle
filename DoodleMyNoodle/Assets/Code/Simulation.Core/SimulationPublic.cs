using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulationPublic
{
    public static void Initialize(ISimModuleBlueprintBank blueprintBank) => SimModules.Initialize(blueprintBank);
    public static void Shutdown() => SimModules.Shutdown();

    /// <summary>
    /// Set the next time id the sim will execute
    /// </summary>
    public static void ForceSetTickId(uint tickId) => SimModules.world.tickId = tickId;

    /// <summary>
    /// Is the simulation ready to run ?
    /// </summary>
    public static bool isInitialized => SimModules.world != null;

    /// <summary>
    /// Can the simulation be ticked ? Some things can prevent the simulation to be ticked (like being in the middle of a scene injection)
    /// </summary>
    public static bool canBeTicked => SimModules.ticker.canSimBeTicked;

    /// <summary>
    /// Can the simulation be saved ? Some things can prevent the simulation to be saved (like being in the middle of a scene injection)
    /// </summary>
    public static bool canBeSaved => SimModules.serializer.canSimWorldBeSaved;

    /// <summary>
    /// The simulation tick delta time. Should be constant for the simulation to stay deterministic
    /// </summary>
    public static readonly Fix64 deltaTime = SimulationConstants.TIME_STEP; // 50 ticks per seconds

    /// <summary>
    /// All registered entites in the simulation
    /// </summary>
    public static List<SimEntity> entities => SimModules.world.entities;

    /// <summary>
    /// The current simulation tick id. Increments by 1 every Tick()
    /// </summary>
    public static uint tickId => SimModules.world.tickId;
    public static void Tick(in SimTickData tickData) => SimModules.ticker.Tick(tickData);

    public static SimBlueprint GetBlueprint(in SimBlueprintId blueprintId) => SimModules.blueprintBank.GetBlueprint(blueprintId);

    public static SimEntity FindEntityWithName(string name) => SimModules.worldSearcher.FindEntityWithName(name);
    public static SimEntity FindEntityWithComponent<T>() => SimModules.worldSearcher.FindEntityWithComponent<T>();
    public static SimEntity FindEntityWithComponent<T>(out T comp) => SimModules.worldSearcher.FindEntityWithComponent(out comp);
}
