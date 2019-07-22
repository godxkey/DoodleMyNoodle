using UnityEngine;
using UnityEngine.SceneManagement;

public class SimModuleSerializer
{
    internal bool canSimWorldBeSaved => 
        SimModules.sceneLoader.pendingSceneLoads == 0
        && SimModules.ticker.isTicking == false; 
}