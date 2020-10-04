using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public static class UIUtility
{
    public static void ResizeGameObjectList<T>(List<T> gameObjectList, int count, T prefab, Transform container, Action<T> onCreate = null, Action<T> onDestroy = null) where T : UnityEngine.Component
    {
        while (gameObjectList.Count < count)
        {
            var newObject = UnityEngine.Object.Instantiate(prefab, container);
            gameObjectList.Add(newObject);
            onCreate?.Invoke(newObject);
        }

        while (gameObjectList.Count > count)
        {
            int i = gameObjectList.Count - 1;
            onDestroy?.Invoke(gameObjectList[i]);
            UnityEngine.Object.Destroy(gameObjectList[i]);
            gameObjectList.RemoveAt(i);
        }
    }

    public static void UpdateGameObjectList<T, U>(List<T> gameObjectList, List<U> data, T prefab, Transform container, Action<T, U> onCreate = null, Action<T, U> onUpdate = null, Action<T> onDeactivate = null) where T : UnityEngine.Component
    {
        int i = 0;
        for (; i < data.Count; i++)
        {
            if(i >= gameObjectList.Count)
            {
                var newObject = UnityEngine.Object.Instantiate(prefab, container);
                gameObjectList.Add(newObject);
                onCreate?.Invoke(newObject, data[i]);
            }

            gameObjectList[i].gameObject.SetActive(true);
            onUpdate?.Invoke(gameObjectList[i], data[i]);
        }

        for (int r = gameObjectList.Count - 1; r >= i; r--)
        {
            onDeactivate?.Invoke(gameObjectList[r]);
            gameObjectList[r].gameObject.SetActive(false);
        }
    }
}