using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimModuleSerializer : IDisposable
{
    internal bool CanSimWorldBeSaved => 
        SimModules.SceneLoader.PendingSceneLoads == 0
        && SimModules.Ticker.IsTicking == false;

    public void Dispose()
    {
    }
}