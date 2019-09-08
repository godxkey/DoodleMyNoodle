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
}
