using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SingletonEntityDataDisplay<T> : GameMonoBehaviour where T : struct, IComponentData
{
    protected static Entity s_singletonData = Entity.Null;

    public override void OnGameUpdate()
    {
        if(SimWorld.HasSingleton<T>())
        {
            Entity currentSingletonData = SimWorld.GetSingletonEntity<T>();
            if(currentSingletonData != s_singletonData)
            {
                s_singletonData = currentSingletonData;
            }
        }
    } 
}
