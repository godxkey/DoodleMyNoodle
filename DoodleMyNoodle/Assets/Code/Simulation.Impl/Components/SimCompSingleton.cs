using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCompSingleton<T> : SimComponent where T : SimCompSingleton<T>
{
    public static T instance { get; private set; }

    public override void OnAddedToEntityList()
    {
        base.OnAddedToEntityList();

        if(instance != null)
        {
            DebugService.LogError($"Singleton [{nameof(T)}] already exists");
        }
        else
        {
            instance = (T)this;
        }
    }

    public override void OnRemovingFromEntityList()
    {
        if(instance == this)
        {
            instance = null;
        }

        base.OnRemovingFromEntityList();
    }
}
