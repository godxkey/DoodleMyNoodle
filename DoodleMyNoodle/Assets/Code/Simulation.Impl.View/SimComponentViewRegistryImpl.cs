using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimComponentViewRegistryImpl
{
    static bool init = false;
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad() // Executed after scene is loaded and game is running
    {
        if (init)
            return;
        init = true;



        // fbessette: Ceci devrait être code-gen. Pour l'instant, on le fait à la main
        SimComponentViewRegistry.RegisterType(typeof(SimComponentTransform2D), typeof(SimComponentViewTransform2D));
    }
}
