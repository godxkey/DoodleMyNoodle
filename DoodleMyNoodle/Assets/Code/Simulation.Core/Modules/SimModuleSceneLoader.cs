using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class SimModuleSceneLoader : SimModuleBase
{
    internal int PendingSceneLoads => _sceneLoadPromises.Count;

    List<ISceneLoadPromise> _sceneLoadPromises = new List<ISceneLoadPromise>();

    /// <summary>
    /// Instantiate all gameobjects in the given scene and inject them all into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal void LoadScene(string sceneName)
    {
        // fbessette: eventually, we'll want to have a preloading mechanism outside of the simulation so we don't have a CPU spike here.
        _sceneLoadPromises.Add(SceneService.Load(sceneName, LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));
    }

    internal void Update()
    {
        // this makes sure we apply the scene in the order we received the commands
        while (_sceneLoadPromises.Count > 0 && _sceneLoadPromises.First().IsComplete)
        {
            ApplyLoadedScene(_sceneLoadPromises.First().Scene);
            _sceneLoadPromises.RemoveFirst();
        }
    }

    void ApplyLoadedScene(Scene scene)
    {
        GameObject[] gameobjects = scene.GetRootGameObjects();

        // fbessette: Apparently, in standalone build, the hierarchy is all desorganised ... sort the gameobjects by name :(
        Array.Sort(gameobjects, (a, b) => string.Compare(a.name, b.name));

        for (int i = 0; i < gameobjects.Length; i++)
        {
            InjectSceneGameObjectIntoSim(gameobjects[i]);
        }
    }

    void InjectSceneGameObjectIntoSim(GameObject gameObject)
    {
        SimEntity simEntity = gameObject.GetComponent<SimEntity>();

        if (simEntity)
        {
            if (simEntity.BlueprintId.IsValid == false)
            {
                Debug.LogError($"Could not inject entity '{simEntity.name}' into sim after loading scene because its blueprintId is invalid. " +
                    $"Try re-saving the scene to fix broken blueprintIds");
                return;
            }

            SimModules._EntityManager.InjectNewEntityIntoSim(simEntity, simEntity.BlueprintId);

            // If the entity has a SimTranform, we should look for child-entities that need to be injected into the sim as well
            SimTransformComponent simTransform = simEntity.SimTransform;
            if (simTransform)
            {
                Transform unityTransform = simEntity.UnityTransform;
                for (int i = 0; i < unityTransform.childCount; i++)
                {
                    InjectSceneGameObjectIntoSim(unityTransform.GetChild(i).gameObject);
                }
            }
        }
    }
}