using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public static class GameObjectExtensions
{
    public static void ToggleActiveState(this GameObject source)
    {
        source.SetActive(!source.activeSelf);
    }

    public static T GetComponentOnlyInChildren<T>(this GameObject source) where T : Component
    {
        foreach (T component in source.GetComponentsInChildren<T>())
        {
            if (component.gameObject != source.gameObject)
            {
                return component;
            }
        }

        return null;
    }

    public static List<GameObject> GetAllChilds(this GameObject source, bool recursive = true)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < source.transform.childCount; i++)
        {
            GameObject currentChild = source.transform.GetChild(i).gameObject;
            list.Add(currentChild);
            if (recursive)
            {
                list.AddRange(GetAllChilds(currentChild));
            }
        }
        return list;
    }
}