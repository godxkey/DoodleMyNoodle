using Sim.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationView : SimulationBase
{
    public static void Initialize(SimulationCoreSettings settings) => SimModules.Initialize(settings);
    public static void Dispose() => SimModules.Dispose();

    /// <summary>
    /// Set the next time id the sim will execute
    /// </summary>
    public static void ForceSetTickId(uint tickId) => SimModules._World.TickId = tickId;
    public static void Tick(in SimTickData tickData) => SimModules._Ticker.Tick(tickData);


    /// <summary>
    /// Can the simulation be ticked ? Some things can prevent the simulation to be ticked (like being in the middle of a scene injection)
    /// </summary>
    public static bool CanBeTicked => SimModules._Ticker.CanSimBeTicked;

    /// <summary>
    /// Update the loading of scenes
    /// </summary>
    public static void UpdateSceneLoads() => SimModules._SceneLoader.Update();

    /// <summary>
    /// Can the simulation be saved ? Some things can prevent the simulation to be saved (like being in the middle of a scene injection)
    /// </summary>
    public static bool CanBeSerialized => SimModules._Serializer.CanSimWorldBeSaved;
    public static bool CanBeDeserialized => SimModules._Serializer.CanSimWorldBeSaved;

    public static SimSerializationOperationWithCache SerializeSimulation() => SimModules._Serializer.SerializeSimulation();
    public static SimDeserializationOperation DeserializeSimulation(string data) => SimModules._Serializer.DeserializeSimulation(data);
}
