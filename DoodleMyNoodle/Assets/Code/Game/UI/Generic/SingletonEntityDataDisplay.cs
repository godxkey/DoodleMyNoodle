using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SingletonEntityDataDisplay<T> : GameMonoBehaviour where T : struct, IComponentData
{
    protected Entity _singletonData = Entity.Null;

    public override void OnGameUpdate()
    {
        if(_singletonData == Entity.Null && SimWorld.HasSingleton<T>())
        {
            _singletonData = SimWorld.GetSingletonEntity<T>();
        }
    } 
}
