using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneExtensions
{
    static public T FindRootObject<T>(this in Scene scene)
    {
        GameObject[] rootObjs = scene.GetRootGameObjects();
        for (int i = 0; i < rootObjs.Length; i++)
        {
            if (rootObjs[i].GetComponent<T>() != null)
                return rootObjs[i].GetComponent<T>();
        }
        return default(T);
    }

    // Guarantees the order of gameobjects by sorting them
    public static GameObject[] GetRootGameObjectsDeterministic(this in Scene scene)
    {
        GameObject[] gameObjects = scene.GetRootGameObjects();

        Array.Sort(gameObjects, (a, b) => string.Compare(a.name, b.name));

        return gameObjects;
    }

}
