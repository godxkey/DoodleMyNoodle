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
        _sceneLoadPromises.Last().OnComplete += OnSceneLoaded;
    }

    void OnSceneLoaded(ISceneLoadPromise sceneLoadPromise)
    {
        _sceneLoadPromises.Remove(sceneLoadPromise);

        GameObject[] gameobjects = sceneLoadPromise.Scene.GetRootGameObjects();

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
            if(simEntity.BlueprintId.IsValid == false)
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

    public override void Dispose()
    {
        base.Dispose();

        for (int i = 0; i < _sceneLoadPromises.Count; i++)
        {
            _sceneLoadPromises[i].OnComplete -= OnSceneLoaded;
        }
        _sceneLoadPromises.Clear();
    }
}