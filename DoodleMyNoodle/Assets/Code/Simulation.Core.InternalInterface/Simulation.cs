using System;
using UnityEngine;

public static class Simulation
{
    /// <summary>
    /// The simulation tick delta time. Should be constant for the simulation to stay deterministic
    /// </summary>
    public static readonly Fix64 deltaTime = SimulationConstants.TIME_STEP; // 50 ticks per seconds

    /// <summary>
    /// The current simulation tick id. Increments by 1 every Tick()
    /// </summary>
    public static uint tickId => SimModules.world.tickId;

    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original) => SimModules.entityManager.Instantiate(original);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original, Transform parent) => SimModules.entityManager.Instantiate(original, parent);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original, in FixVector3 position, in FixQuaternion rotation) => SimModules.entityManager.Instantiate(original, position, rotation);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original, in FixVector3 position, in FixQuaternion rotation, SimTransform parent) => SimModules.entityManager.Instantiate(original, position, rotation, parent);

    /// <summary>
    /// Load the given scene and inject all gameobjects with the SimEntity component into the simulation
    /// </summary>
    public static void LoadScene(string sceneName) => SimModules.sceneLoader.LoadScene(sceneName);

    public static SimBlueprint GetBlueprint(in SimBlueprintId blueprintId) => SimModules.blueprintBank.GetBlueprint(blueprintId);

    public static SimEntity FindEntityWithName(string name) => SimModules.worldSearcher.FindEntityWithName(name);
    public static SimEntity FindEntityWithComponent<T>() => SimModules.worldSearcher.FindEntityWithComponent<T>();
    public static SimEntity FindEntityWithComponent<T>(out T comp) => SimModules.worldSearcher.FindEntityWithComponent(out comp);
    public static void ForEveryEntityWithComponent<T>(Action<T> action) => SimModules.worldSearcher.ForEveryEntityWithComponent(action);
    /// <summary>
    /// Return false to stop the iteration
    /// </summary>
    public static void ForEveryEntityWithComponent<T>(Func<T, bool> action) => SimModules.worldSearcher.ForEveryEntityWithComponent(action);

    public static class Random
    {
        public static int Int() => SimModules.random.RandomInt();
        public static uint UInt() => SimModules.random.RandomUInt();
        public static bool Bool() => SimModules.random.RandomBool();
        public static Fix64 Range01() => SimModules.random.Random01();
        public static Fix64 Range(in Fix64 min, in Fix64 max) => SimModules.random.RandomRange(min, max);
        /// <summary>
        /// Vector will be normalized
        /// </summary>
        public static FixVector2 Direction2D() => SimModules.random.RandomDirection2D();
        /// <summary>
        /// Vector will be normalized
        /// </summary>
        public static FixVector3 Direction3D() => SimModules.random.RandomDirection3D();
    }
}
