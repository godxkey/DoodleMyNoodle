using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public static class TransformExtensions
{
    public static List<Transform> GetAllChildren(this Transform source, bool recursive = true)
    {
        List<Transform> list = new List<Transform>(Mathf.Max(source.childCount * 4, 16));

        GetAllChildren(source, list, recursive);

        return list;
    }

    public static void GetAllChildren(this Transform source, List<Transform> children, bool recursive = true)
    {
        int c = source.childCount;
        for (int i = 0; i < c; i++)
        {
            Transform currentChild = source.GetChild(i);
            children.Add(currentChild);

            if (recursive)
            {
                GetAllChildren(currentChild, children);
            }
        }
    }
}