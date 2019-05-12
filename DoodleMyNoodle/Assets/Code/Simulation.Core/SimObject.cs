using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimObject
{
    internal SimObject()
    {

    }

    public SimWorld world { get; internal set; }


    public SimEntity InstantiateEntity()
    {
        return world.InstantiateEntity();
    }

    public virtual void OnAwake() { }
    //public virtual void OnStart() { } not implemented yet
    public virtual void OnDestroy() { }
}
