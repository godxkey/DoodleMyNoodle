using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoreServiceManagerBootstrap
{
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad() // Executed after scene is loaded and game is running
    {
        if (CoreServiceManager.Instance == null)
        {
            new CoreServiceManager();
        }
    }
}
