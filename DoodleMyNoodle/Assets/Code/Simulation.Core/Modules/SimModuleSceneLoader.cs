using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimModuleSceneLoader : IDisposable
{
    internal int PendingSceneLoads = 0;

    public void Dispose()
    {

    }

    /// <summary>
    /// Instantiate all gameobjects in the given scene and inject them all into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal void LoadScene(string sceneName)
    {
        // fbessette: eventually, we'll want to have a preloading mechanism outside of the simulation so we don't have a CPU spike here.
        PendingSceneLoads++;
        SceneService.Load(sceneName, LoadSceneMode.Additive, (scene) =>
        {
            PendingSceneLoads--;

            GameObject[] gameobjects = scene.GetRootGameObjects();

            // fbessette: Apparently, in standalone build, the hierarchy is all desorganised ... sort the gameobjects by name :(

            Array.Sort(gameobjects, (a, b) => string.Compare(a.name, b.name));

            for (int i = 0; i < gameobjects.Length; i++)
            {
                SimEntity newEntity = gameobjects[i].GetComponent<SimEntity>();
                if (newEntity)
                {
                    SimModules.EntityManager.InjectNewEntityIntoSim(newEntity, new SimBlueprintId(SimBlueprintId.BlueprintType.SceneGameObject, "TODO"));
                }
            }
        });
    }
}