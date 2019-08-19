using UnityEngine;
using UnityEngine.SceneManagement;

public class SimModuleSceneLoader
{
    internal int pendingSceneLoads = 0;

    /// <summary>
    /// Instantiate all gameobjects in the given scene and inject them all into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal void LoadScene(string sceneName)
    {
        // fbessette: eventually, we'll want to have a preloading mechanism outside of the simulation so we don't have a CPU spike here.
        pendingSceneLoads++;
        SceneService.Load(sceneName, LoadSceneMode.Additive, (scene) =>
        {
            pendingSceneLoads--;

            GameObject[] gameobjects = scene.GetRootGameObjects();

            // fbessette: Apparently, in standalone build, the hierarchy is reversed ... Do a reverse loop
#if UNITY_EDITOR
            for (int i = 0; i < gameobjects.Length; i++)
#else
            for (int i = gameobjects.Length - 1; i >= 0; i--)
#endif
            {
                SimEntity newEntity = gameobjects[i].GetComponent<SimEntity>();
                if (newEntity)
                {
                    SimModules.entityManager.InjectNewEntityIntoSim(newEntity, new SimBlueprintId(SimBlueprintId.Type.SceneGameObject, "TODO"));
                }
            }
        });
    }

}