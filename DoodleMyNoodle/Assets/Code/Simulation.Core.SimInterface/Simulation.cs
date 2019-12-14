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

    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static T Instantiate<T>(T component) where T : SimComponent
        => SimModules._EntityManager.Instantiate(component.SimEntity).GetComponent<T>();
    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static T Instantiate<T>(T component, SimTransformComponent parent) where T : SimComponent
        => SimModules._EntityManager.Instantiate(component.SimEntity, parent).GetComponent<T>();
    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static T Instantiate<T>(T component, in FixVector3 position, in FixQuaternion rotation) where T : SimComponent
        => SimModules._EntityManager.Instantiate(component.SimEntity, in position, in rotation).GetComponent<T>();
    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static T Instantiate<T>(T component, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent) where T : SimComponent
        => SimModules._EntityManager.Instantiate(component.SimEntity, in position, in rotation, parent).GetComponent<T>();

    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(in SimBlueprint original) 
        => SimModules._EntityManager.Instantiate(original);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(in SimBlueprint original, SimTransformComponent parent) 
        => SimModules._EntityManager.Instantiate(original, parent);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(in SimBlueprint original, in FixVector3 position, in FixQuaternion rotation) 
        => SimModules._EntityManager.Instantiate(original, position, rotation);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    public static SimEntity Instantiate(in SimBlueprint original, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent) 
        => SimModules._EntityManager.Instantiate(original, position, rotation, parent);

    public static void Destroy(SimEntity simEntity)
        => SimModules._EntityManager.Destroy(simEntity);
    public static void Destroy<T>(T component) where T : SimComponent
        => SimModules._EntityManager.Destroy(component.SimEntity);
    
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
