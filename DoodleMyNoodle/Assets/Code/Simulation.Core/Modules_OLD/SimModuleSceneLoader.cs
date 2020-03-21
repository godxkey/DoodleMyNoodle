using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class SimModuleSceneLoader : SimModuleBase
{
    internal int PendingSceneLoads => _sceneLoads.Count;

    struct SceneLoads
    {
        public ISceneLoadPromise SceneLoadPromise;
        public World SimulationWorld;
    }

    List<SceneLoads> _sceneLoads = new List<SceneLoads>();

#if UNITY_EDITOR
    public static Action<Scene> s_EditorValidationMethod;
#endif

    /// <summary>
    /// Instantiate all gameobjects in the given scene and inject them all into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal void LoadScene(string sceneName)
    {
        // fbessette: eventually, we'll want to have a preloading mechanism outside of the simulation so we don't have a CPU spike here.
        _sceneLoads.Add(new SceneLoads()
        {
            SceneLoadPromise = SceneService.Load(sceneName, LoadSceneMode.Additive, LocalPhysicsMode.Physics3D),
            SimulationWorld = null
        });

    }

    internal void Update()
    {
        // this makes sure we apply the scene in the order we received the commands
        while (_sceneLoads.Count > 0 && _sceneLoads.First().SceneLoadPromise.IsComplete)
        {
            ApplyLoadedScene(_sceneLoads.First().SceneLoadPromise.Scene);
            _sceneLoads.RemoveFirst();
        }
    }

    void ApplyLoadedScene(Scene scene)
    {
        GameObject[] gameobjects = scene.GetRootGameObjects();

#if UNITY_EDITOR
        s_EditorValidationMethod?.Invoke(scene);
#endif

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
            if (!simEntity.BlueprintId.IsValid)
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
