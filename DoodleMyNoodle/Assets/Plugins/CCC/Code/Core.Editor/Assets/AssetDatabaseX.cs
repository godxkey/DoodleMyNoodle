using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AssetDatabaseX
{
    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        List<T> assets = new List<T>(guids.Length);
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }

    public static List<GameObject> FindPrefabsAssets()
    {
        string[] guids = AssetDatabase.FindAssets($"t:prefab");
        List<GameObject> assets = new List<GameObject>(guids.Length);
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }

    public static void FindPrefabsAssets(out List<KeyValuePair<string, GameObject>> result)
    {
        string[] guids = AssetDatabase.FindAssets($"t:prefab");
        result = new List<KeyValuePair<string, GameObject>>(guids.Length);
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (asset != null)
            {
                result.Add(new KeyValuePair<string, GameObject>(guids[i], asset));
            }
        }
    }

    // DOES NOT DEEP SEARCH INSIDE THE PREFAB
    public static List<GameObject> FindPrefabsAssetsWithComponent<T>() where T : UnityEngine.Component
    {
        List<GameObject> prefabAssets = FindPrefabsAssets();

        for (int i = 0; i < prefabAssets.Count; i++)
        {
            if(prefabAssets[i].GetComponent<T>() == null)
            {
                prefabAssets.RemoveWithLastSwapAt(i);
                i--;
            }
        }

        return prefabAssets;
    }

    // DOES NOT DEEP SEARCH INSIDE THE PREFAB
    public static void FindPrefabsAssetsWithComponent<T>(out List<KeyValuePair<string, GameObject>> result) where T : UnityEngine.Component
    {
        FindPrefabsAssets(out result);

        for (int i = 0; i < result.Count; i++)
        {
            if (result[i].Value.GetComponent<T>() == null)
            {
                result.RemoveWithLastSwapAt(i);
                i--;
            }
        }
    }
}