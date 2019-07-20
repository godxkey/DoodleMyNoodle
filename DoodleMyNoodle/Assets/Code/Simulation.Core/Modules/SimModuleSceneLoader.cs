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
            foreach (GameObject gameObject in scene.GetRootGameObjects())
            {
                SimEntity newEntity = gameObject.GetComponent<SimEntity>();
                if (newEntity)
                {
                    SimModules.entityManager.InjectNewEntityIntoSim(newEntity, new SimBlueprintId(SimBlueprintId.Type.SceneGameObject, "TODO"));
                }
            }
        });
    }

}