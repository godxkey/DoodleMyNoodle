using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngineX;

public class GridGenerator
{
    private const string PATH = "Assets/Prefabs/Game/Entities/";

    [MenuItem("Tools/Generate Grid Files")]
    public static void Generate()
    {
        GameObject newSimulationGridPrefab = new GameObject();
        newSimulationGridPrefab.AddComponent<SimAsset>();
        newSimulationGridPrefab.GetComponent<SimAsset>().HasTransform = true;
        newSimulationGridPrefab.GetComponent<SimAsset>().Editor_SetShowGhost(false);
        newSimulationGridPrefab.GetComponent<SimAsset>().ViewTechType = ViewTechType.GameObject;

        GameObject newViewGridPrefab = new GameObject();
        newViewGridPrefab.AddComponent<Grid>();
        newViewGridPrefab.GetComponent<Grid>().cellSize = new Vector3(1, 1, 0);
        newViewGridPrefab.GetComponent<Grid>().cellGap = new Vector3(0, 0, 0);
        newViewGridPrefab.GetComponent<Grid>().cellLayout = GridLayout.CellLayout.Rectangle;
        newViewGridPrefab.GetComponent<Grid>().cellSwizzle = GridLayout.CellSwizzle.XYZ;

        Scene activeScene = EditorSceneManager.GetActiveScene();
        GameObject[] rootGameObjects = activeScene.GetRootGameObjects();
        GameObject sceneGrid = null;
        foreach (GameObject gameObject in rootGameObjects)
        {
            if (gameObject.HasComponent<Grid>())
            {
                sceneGrid = gameObject;
                SetupSceneGrid(gameObject);

                TilemapRenderer[] tilemapRenderers = gameObject.GetComponentsInChildren<TilemapRenderer>();
                foreach (TilemapRenderer tileMapRenderer in tilemapRenderers)
                {
                    // Simulation Grid doesn't need to be copied in a prefab
                    if (tileMapRenderer.sortingLayerName == GameConstants.GRID_SIMULATION_LAYER_NAME)
                    {
                        continue;
                    }

                    Tilemap editorTilemap = tileMapRenderer.GetComponent<Tilemap>();
                    if (tileMapRenderer == null || editorTilemap == null)
                    {
                        Debug.LogError("Grid is invalid since there's no tilemap or tilemaprenderer");
                        continue;
                    }

                    GameObject tileMapCopy = CopySceneTileMap(tileMapRenderer, editorTilemap);
                    tileMapCopy.transform.parent = newViewGridPrefab.transform;
                }

                break; // only do one grid
            }
        }

        PrefabUtility.SaveAsPrefabAsset(newSimulationGridPrefab, GetGeneratedFilePath());
        newSimulationGridPrefab.DestroyImmediate();
        
        PrefabUtility.SaveAsPrefabAsset(newViewGridPrefab, GetGeneratedFilePath(true));
        newViewGridPrefab.DestroyImmediate();
        GameObject createdSimulationPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetGeneratedFilePath());
        if (createdSimulationPrefab != null)
        {
            SimAsset simAsset = createdSimulationPrefab.GetComponent<SimAsset>();
            if (simAsset != null)
            {
                createdSimulationPrefab.GetComponent<SimAsset>().Editor_SetBindedViewPrefab((GameObject)AssetDatabase.LoadAssetAtPath(GetGeneratedFilePath(true), typeof(GameObject)));
                EditorUtility.SetDirty(createdSimulationPrefab);
            }

            if (sceneGrid != null && simAsset != null && sceneGrid.HasComponent<LevelGridAuth>())
            {
                sceneGrid.GetComponent<LevelGridAuth>().PrefabSimAsset = simAsset;
            }
        }

        EditorSceneManager.MarkSceneDirty(activeScene);
        AssetDatabase.SaveAssets();
    }

    private static string GetGeneratedFilePath(bool isForView = false)
    {
        string prefix = isForView ? "VE" : "SE";
        string category = isForView ? "ViewEntity" : "SimulationEntity";
        string sceneName = EditorSceneManager.GetActiveScene().name;
        string fileName =  $"{prefix}_Grid_{sceneName}";
        return  $"{PATH}{category}/Grids/{fileName}.prefab";
    }

    private static void SetupSceneGrid(GameObject gridGameObject)
    {
        if (!gridGameObject.HasComponent<ConvertToSimEntity>())
        {
            gridGameObject.AddComponent<ConvertToSimEntity>();
        }

        Grid gridComponent = gridGameObject.HasComponent<Grid>() ? gridGameObject.GetComponent<Grid>() : gridGameObject.AddComponent<Grid>();
        gridComponent.cellSize = new Vector3(1, 1, 0);
        gridComponent.cellGap = new Vector3(0, 0, 0);
        gridComponent.cellLayout = GridLayout.CellLayout.Rectangle;
        gridComponent.cellSwizzle = GridLayout.CellSwizzle.XYZ;

        LevelGridAuth levelGridAuth = gridGameObject.HasComponent<LevelGridAuth>() ? gridGameObject.GetComponent<LevelGridAuth>() : gridGameObject.AddComponent<LevelGridAuth>();
        levelGridAuth.Grid = gridComponent;

        LevelGridSettings GlobalGridSettings = AssetDatabase.LoadAssetAtPath<LevelGridSettings>("Assets/ScriptableObjects/Game/GridSettings/GridSettings.asset");
        if (GlobalGridSettings == null)
        {
            Debug.LogError("Global Grid Settings missing (GridSettings.asset), please create the file in Assets/ScriptableObjects/Game/GridSettings");
            return;
        }
        levelGridAuth.GlobalGridSettings = GlobalGridSettings;
    }

    private static GameObject CopySceneTileMap(TilemapRenderer editorTileMapRenderer, Tilemap editorTilemap)
    {
        GameObject tileMapCopy = new GameObject();
        tileMapCopy.name = editorTileMapRenderer.gameObject.name;

        TilemapRenderer newTilemapRenderer = tileMapCopy.AddComponent<TilemapRenderer>();
        if (newTilemapRenderer != null)
        {
            newTilemapRenderer.sortingLayerID = editorTileMapRenderer.sortingLayerID;
            newTilemapRenderer.sortingOrder = editorTileMapRenderer.sortingOrder;
        }

        Tilemap newTilemap = tileMapCopy.GetComponent<Tilemap>();
        newTilemap.size = editorTilemap.size;
        newTilemap.origin = editorTilemap.origin;
        if (newTilemap != null)
        {
            for (int i = 0; i < editorTilemap.size.x; i++)
            {
                for (int j = 0; j < editorTilemap.size.y; j++)
                {
                    Vector3Int currentTilePos = new Vector3Int(editorTilemap.origin.x + i, editorTilemap.origin.y + j, 0);
                    TileBase currentTile = editorTilemap.GetTile(currentTilePos);
                    if (currentTile != null)
                    {
                        newTilemap.SetTile(currentTilePos, currentTile);
                    }
                }
            }
        }

        return tileMapCopy;
    }
}