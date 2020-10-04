using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public static class UIUtility
{
    public static void ResizeGameObjectList<T>(List<T> componentList, int count, T prefab, Transform container, Action<T> onCreate = null, Action<T> onDestroy = null) where T : UnityEngine.Component
    {
        while (componentList.Count < count)
        {
            var newObject = UnityEngine.Object.Instantiate(prefab, container);
            componentList.Add(newObject);
            onCreate?.Invoke(newObject);
        }

        while (componentList.Count > count)
        {
            int i = componentList.Count - 1;
            onDestroy?.Invoke(componentList[i]);
            UnityEngine.Object.Destroy(componentList[i].gameObject);
            componentList.RemoveAt(i);
        }
    }

    public static void UpdateGameObjectList<T, U>(List<T> componentList, List<U> data, T prefab, Transform container, Action<T, U> onCreate = null, Action<T, U> onUpdate = null, Action<T> onDeactivate = null) where T : UnityEngine.Component
    {
        int i = 0;
        for (; i < data.Count; i++)
        {
            if(i >= componentList.Count)
            {
                var newObject = UnityEngine.Object.Instantiate(prefab, container);
                componentList.Add(newObject);
                onCreate?.Invoke(newObject, data[i]);
            }

            componentList[i].gameObject.SetActive(true);
            onUpdate?.Invoke(componentList[i], data[i]);
        }

        for (int r = componentList.Count - 1; r >= i; r--)
        {
            onDeactivate?.Invoke(componentList[r]);
            componentList[r].gameObject.SetActive(false);
        }
    }
}