using Sim.Operations;
using Unity.Entities;

public class SimulationView : SimulationBase
{
    public static void Initialize(SimulationCoreSettings settings) => SimModules.Initialize(settings);
    public static void Dispose() => SimModules.Dispose();

    /// <summary>
    /// Set the next time id the sim will execute
    /// </summary>
    public static void ForceSetTickId(uint tickId) => SimModules._World.TickId = tickId;
    public static void Tick(in SimTickDataOld tickData) => SimModules._Ticker.Tick(tickData);

    /// <summary>
    /// Can the simulation be ticked ? Some things can prevent the simulation to be ticked (like being in the middle of a scene injection)
    /// </summary>
    public static bool CanBeTicked => SimModules._Ticker.CanSimBeTicked;

    /// <summary>
    /// Update the loading of scenes
    /// </summary>
    public static void UpdateSceneLoads() => SimModules._SceneLoader.Update();

    ///// <summary>
    ///// Can the simulation be saved ? Some things can prevent the simulation to be saved (like being in the middle of a scene injection)
    ///// </summary>
    //public static bool CanBeSerialized => SimModules._Serializer.CanSimWorldBeSaved;
    //public static bool CanBeDeserialized => SimModules._Serializer.CanSimWorldBeSaved;

    public static SimSerializationOperationWithCache SerializeSimulation(World simulationWorld) => SimModuleSerializer.SerializeSimulation(simulationWorld);
    public static SimDeserializationOperation DeserializeSimulation(string data, World simulationWorld) => SimModuleSerializer.DeserializeSimulation(data, simulationWorld);
}
