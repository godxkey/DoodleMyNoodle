using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngineX;

[CustomEditor(typeof(LevelGridAuth))]
public class LevelGridAuthEditor : Editor
{
    private class TilemapCache
    {
        public TileBase[] AllTiles;
        public BoundsInt Bounds;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelGridAuth castedTarget = (LevelGridAuth)target;

        if (castedTarget.gameObject.scene.IsValid())
        {
            if (GUILayout.Button("Revert Unwanted Overrides"))
                RevertUnwantedOverrides(castedTarget);


            if (GUILayout.Button(castedTarget.PrefabSimAsset == null ? "Generate Asset Files" : "Update Asset Files"))
                GenerateAssetFiles(castedTarget);
        }
    }

    private static void RevertUnwantedOverrides(LevelGridAuth castedTarget)
    {
        Transform transform = castedTarget.transform;

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Cache Data
        ////////////////////////////////////////////////////////////////////////////////////////
        SimAsset simAsset = castedTarget.PrefabSimAsset;

        // Tilemaps
        TilemapCache[] tilemapCaches = new TilemapCache[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            var tilemap = transform.GetChild(i).GetComponent<Tilemap>();
            if (tilemap != null)
                tilemapCaches[i] = CacheTilemap(tilemap);
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Revert
        ////////////////////////////////////////////////////////////////////////////////////////
        PrefabUtility.SetPropertyModifications(castedTarget.gameObject, new PropertyModification[0]);

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Restore
        ////////////////////////////////////////////////////////////////////////////////////////
        castedTarget.PrefabSimAsset = simAsset;

        // Tilemaps
        for (int i = 0; i < transform.childCount; i++)
        {
            var tilemap = transform.GetChild(i).GetComponent<Tilemap>();
            if (tilemap != null && i < tilemapCaches.Length)
            {
                RestoreTilemap(tilemap, tilemapCaches[i]);
            }
        }

        EditorSceneManager.MarkSceneDirty(castedTarget.gameObject.scene);
    }

    private static void RestoreTilemap(Tilemap tilemap, TilemapCache cache)
    {
        tilemap.SetTilesBlock(cache.Bounds, cache.AllTiles);
    }

    private static TilemapCache CacheTilemap(Tilemap tilemap)
    {
        return new TilemapCache()
        {
            AllTiles = tilemap.GetTilesBlock(tilemap.cellBounds),
            Bounds = tilemap.cellBounds
        };
    }

    private static void GenerateAssetFiles(LevelGridAuth castedTarget)
    {
        GridGenerator.Generate(castedTarget);
    }
}
