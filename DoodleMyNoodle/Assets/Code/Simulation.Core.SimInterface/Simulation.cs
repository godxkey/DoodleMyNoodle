using System;
using UnityEngine;

public class Simulation : SimulationBase
{
    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimEntity entity)
        => SimModules._EntityManager.Instantiate(entity);
    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimEntity entity, SimTransformComponent parent)
        => SimModules._EntityManager.Instantiate(entity, parent);
    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimEntity entity, in FixVector3 position, in FixQuaternion rotation)
        => SimModules._EntityManager.Instantiate(entity, in position, in rotation);
    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimEntity entity, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent)
        => SimModules._EntityManager.Instantiate(entity, in position, in rotation, parent);

    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //public static SimEntity Instantiate(in SimBlueprintId blueprintId)
    //    => SimModules._EntityManager.Instantiate(blueprintId);
    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //public static SimEntity Instantiate(in SimBlueprintId blueprintId, Transform parent)
    //    => SimModules._EntityManager.Instantiate(in blueprintId, parent);
    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //public static SimEntity Instantiate(in SimBlueprintId blueprintId, in FixVector3 position, in FixQuaternion rotation)
    //    => SimModules._EntityManager.Instantiate(in blueprintId, in position, in rotation);
    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //public static SimEntity Instantiate(in SimBlueprintId blueprintId, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent)
    //    => SimModules._EntityManager.Instantiate(in blueprintId, in position, in rotation, parent);
    
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original) 
        => SimModules._EntityManager.Instantiate(original);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original, SimTransformComponent parent) 
        => SimModules._EntityManager.Instantiate(original, parent);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original, in FixVector3 position, in FixQuaternion rotation) 
        => SimModules._EntityManager.Instantiate(original, position, rotation);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(SimBlueprint original, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent) 
        => SimModules._EntityManager.Instantiate(original, position, rotation, parent);

    public static void Destroy(SimEntity simEntity)
        => SimModules._EntityManager.Destroy(simEntity);
    
    /// <summary>
    /// Load the given scene and inject all gameobjects with the SimEntity component into the simulation
    /// </summary>
    public static void LoadScene(string sceneName) => SimModules._SceneLoader.LoadScene(sceneName);

    public static class Random
    {
        public static int Int() => SimModules._Random.RandomInt();
        public static uint UInt() => SimModules._Random.RandomUInt();
        public static bool Bool() => SimModules._Random.RandomBool();
        public static Fix64 Range01() => SimModules._Random.Random01();
        public static Fix64 Range(in Fix64 min, in Fix64 max) => SimModules._Random.RandomRange(min, max);
        /// <summary>
        /// Vector will be normalized
        /// </summary>
        public static FixVector2 Direction2D() => SimModules._Random.RandomDirection2D();
        /// <summary>
        /// Vector will be normalized
        /// </summary>
        public static FixVector3 Direction3D() => SimModules._Random.RandomDirection3D();
    }
}
