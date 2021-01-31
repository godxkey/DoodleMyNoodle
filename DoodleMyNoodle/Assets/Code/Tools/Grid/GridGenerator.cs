using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngineX;

public class GridGenerator
{
    private const string PATH = "Assets/Prefabs/Game/Entities/";

    [MenuItem("Tools/Generate Grid Files")]
    public static void Generate()
    {
        GameObject newSimulationGridPrefab = new GameObject();
        newSimulationGridPrefab.AddComponent<Grid>();
        newSimulationGridPrefab.GetComponent<Grid>().cellSize = new Vector3(1, 1, 0);
        newSimulationGridPrefab.GetComponent<Grid>().cellGap = new Vector3(0, 0, 0);
        newSimulationGridPrefab.GetComponent<Grid>().cellLayout = GridLayout.CellLayout.Rectangle;
        newSimulationGridPrefab.GetComponent<Grid>().cellSwizzle = GridLayout.CellSwizzle.XYZ;

        newSimulationGridPrefab.AddComponent<SimAsset>();
        newSimulationGridPrefab.GetComponent<SimAsset>().HasTransform = true;
        newSimulationGridPrefab.GetComponent<SimAsset>().ViewTechType = ViewTechType.GameObject;

        newSimulationGridPrefab.AddComponent<LevelGridAuth>();
        newSimulationGridPrefab.GetComponent<LevelGridAuth>().Grid = newSimulationGridPrefab.GetComponent<Grid>();

        LevelGridSettings GlobalGridSettings = AssetDatabase.LoadAssetAtPath<LevelGridSettings>("Assets/ScriptableObjects/Game/GridSettings/GridSettings.asset");
        if (GlobalGridSettings == null)
        {
            Debug.LogError("Global Grid Settings missing (GridSettings.asset), please create the file in Assets/ScriptableObjects/Game/GridSettings");
            return;
        }
        newSimulationGridPrefab.GetComponent<LevelGridAuth>().GlobalGridSettings = GlobalGridSettings;

        GameObject newViewGridPrefab = new GameObject();
        newViewGridPrefab.AddComponent<Grid>();
        newViewGridPrefab.GetComponent<Grid>().cellSize = new Vector3(1, 1, 0);
        newViewGridPrefab.GetComponent<Grid>().cellGap = new Vector3(0, 0, 0);
        newViewGridPrefab.GetComponent<Grid>().cellLayout = GridLayout.CellLayout.Rectangle;
        newViewGridPrefab.GetComponent<Grid>().cellSwizzle = GridLayout.CellSwizzle.XYZ;

        GameObject[] RootGameObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject gameObject in RootGameObjects)
        {
            if (gameObject.HasComponent<Grid>())
            {
                TilemapRenderer[] tilemapRenderers = gameObject.GetComponentsInChildren<TilemapRenderer>();
                foreach (TilemapRenderer tileMap in tilemapRenderers)
                {
                    GameObject tileMapCopy = new GameObject();
                    tileMapCopy.name = tileMap.gameObject.name;
                    CopyComponent(tileMap, tileMapCopy);
                    CopyComponent(tileMap.gameObject.GetComponent<Tilemap>(), tileMapCopy);
                    tileMapCopy.transform.parent = tileMap.sortingLayerName == "Grid_Addons" ? newSimulationGridPrefab.transform : newViewGridPrefab.transform;
                }

                break; // only do one grid
            }
        }

        PrefabUtility.SaveAsPrefabAsset(newSimulationGridPrefab, GetGeneratedFilePath());
        newSimulationGridPrefab.DestroyImmediate();

        PrefabUtility.SaveAsPrefabAsset(newViewGridPrefab, GetGeneratedFilePath(true));
        newViewGridPrefab.DestroyImmediate();
    }

    private static string GetGeneratedFilePath(bool isForView = false)
    {
        string category = isForView ? "ViewEntity" : "SimulationEntity";
        string sceneName = EditorSceneManager.GetActiveScene().name;
        string fileName = isForView ? "VE_" + sceneName + "_Grid" : "SE_" + sceneName + "_Grid";
        return PATH + category + "/Grids/" + fileName + ".prefab";
    }

    static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;

        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }

        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }

        return dst as T;
    }
}