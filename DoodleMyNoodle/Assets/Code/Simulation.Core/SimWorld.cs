using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SimWorld : IDisposable
{
    List<SimObject> simObjects;

    public SimEntity InstantiateEntity()
    {
        return (SimEntity)OnCreate(new SimEntity());
    }

    internal T Create<T>() where T : SimObject,  new()
    {
        return (T)OnCreate(new T());
    }

    SimObject OnCreate(SimObject obj)
    {
        obj.world = this;

        simObjects.Add(obj);

        obj.OnAwake();

        return obj;
    }

    internal void Destroy(SimObject obj)
    {
        int i = simObjects.IndexOf(obj);
        if (i >= 0)
            DestroyAt(i);
    }

    void DestroyAt(int index)
    {
        simObjects[index].OnDestroy();
        simObjects.RemoveAt(index);
    }

    public virtual void Tick_PreInput() { }
    public virtual void Tick_PostInput() { }

    public void Dispose()
    {
        for (int i = simObjects.Count - 1; i >= 0; i--)
        {
            DestroyAt(i);
        }
    }
}
