using UnityEngine;

/// <typeparam name="T">yourself</typeparam>
public abstract class SelfSpawningSingleton<T> : Singleton<T> where T : SelfSpawningSingleton<T>
{
    protected static T _Instance
    {
        get
        {
            SpawnInstanceIfNotSpawned();
            return instance;
        }
    }

    protected static T GetRawInstance()
    {
        return instance;
    }

    protected static void SpawnInstanceIfNotSpawned()
    {
        if (instance == null)
        {
            var obj = new GameObject("t");
            obj.AddComponent<T>();
            obj.name = instance.GameObjectName();
        }
    }

    protected abstract string GameObjectName();
}
