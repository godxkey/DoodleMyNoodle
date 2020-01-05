using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimEventSingleton<ChildClass> : SimEventComponent
    where ChildClass : SimEventSingleton<ChildClass>
{
    [field: System.NonSerialized]
    public static ChildClass Instance { get; private set; }

    public override void OnAddedToRuntime()
    {
        if (Instance != null)
        {
            DebugService.LogError($"Singleton [{nameof(ChildClass)}] already exists");
        }
        else
        {
            Instance = (ChildClass)this;
        }
    }

    public override void OnRemovingFromRuntime()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        base.OnRemovingFromRuntime();
    }
}
