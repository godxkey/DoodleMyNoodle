using System;
using UnityEngine;

public class Simulation : SimulationPublic
{
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
    public static SimEntity Instantiate(SimBlueprint original, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent) => SimModules.entityManager.Instantiate(original, position, rotation, parent);

    /// <summary>
    /// Load the given scene and inject all gameobjects with the SimEntity component into the simulation
    /// </summary>
    public static void LoadScene(string sceneName) => SimModules.sceneLoader.LoadScene(sceneName);

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
