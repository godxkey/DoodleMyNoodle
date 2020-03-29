using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static bool GetComponent<T>(this GameObject gameObject, out T result)
    {
        result = gameObject.GetComponent<T>();
        return result != null;
    }

    public static bool HasComponent<T>(this GameObject gameObject)
    {
        return gameObject.GetComponent<T>() != null;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T result = gameObject.GetComponent<T>();
        if (!result)
            result = gameObject.AddComponent<T>();
        return result;
    }
}
