using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngineX;

public class GridGenerator
{
    private const string PATH = "Assets/GameContent/Levels/Grids";

    [MenuItem("Tools/Generate Grid Files")]
    public static void Generate()
    {
        Generate(EditorSceneManager.GetActiveScene().FindComponentOnRoots<LevelGridAuth>());
    }

    public static void Generate(LevelGridAuth levelGridAuth)
    {
        if (levelGridAuth == null)
        {
            Log.Error($"No {nameof(LevelGridAuth)} found.");
            return;
        }

        if (!levelGridAuth.gameObject.scene.IsValid())
        {
            Log.Error("Can only generate assets for levelGridAuth in scenes");
            return;
        }

        GameObject sceneGrid = levelGridAuth.gameObject;

        string simAssetPath = GetGeneratedFilePath(sceneGrid, isForView: false);
        string viewAssetPath = GetGeneratedFilePath(sceneGrid, isForView: true);

        // Create & Save Assets
        {
            GameObject simGridPrefab = new GameObject();
            simGridPrefab.AddComponent<SimAsset>();
            simGridPrefab.GetComponent<SimAsset>().HasTransform = true;
            simGridPrefab.GetComponent<SimAsset>().Editor_SetShowGhost(false);
            simGridPrefab.GetComponent<SimAsset>().ViewTechType = ViewTechType.GameObject;

            GameObject viewGridPrefab = new GameObject();
            viewGridPrefab.AddComponent<Grid>();
            viewGridPrefab.GetComponent<Grid>().cellSize = new Vector3(1, 1, 0);
            viewGridPrefab.GetComponent<Grid>().cellGap = new Vector3(0, 0, 0);
            viewGridPrefab.GetComponent<Grid>().cellLayout = GridLayout.CellLayout.Rectangle;
            viewGridPrefab.GetComponent<Grid>().cellSwizzle = GridLayout.CellSwizzle.XYZ;

            SetupSceneGrid(sceneGrid);

            foreach (TilemapRenderer tilemapRenderer in sceneGrid.GetComponentsInChildren<TilemapRenderer>())
            {
                // Simulation Grid doesn't need to be copied in a prefab
                if (tilemapRenderer.sortingLayerName == GameConstants.LAYER_GRID_SIMULATION)
                {
                    continue;
                }

                Tilemap tilemap = tilemapRenderer.GetComponent<Tilemap>();
                if (tilemapRenderer == null || tilemap == null)
                {
                    Log.Error("Grid is invalid since there's no tilemap or tilemaprenderer");
                    continue;
                }

                GameObject tilemapCopy = CloneTilemap(tilemapRenderer, tilemap);
                tilemapCopy.transform.parent = viewGridPrefab.transform;
            }

            PrefabUtility.SaveAsPrefabAsset(simGridPrefab, simAssetPath);
            simGridPrefab.DestroyImmediate();

            PrefabUtility.SaveAsPrefabAsset(viewGridPrefab, viewAssetPath);
            viewGridPrefab.DestroyImmediate();
        }

        // Bind assets
        {
            GameObject simPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(simAssetPath);
            GameObject viewPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(viewAssetPath);
            simPrefab.GetComponent<SimAsset>().Editor_SetBindedViewPrefab(viewPrefab);
            EditorUtility.SetDirty(simPrefab);

            sceneGrid.GetComponent<LevelGridAuth>().PrefabSimAsset = simPrefab.GetComponent<SimAsset>();
            PrefabUtility.RecordPrefabInstancePropertyModifications(sceneGrid.GetComponent<LevelGridAuth>());
        }

        EditorSceneManager.MarkSceneDirty(sceneGrid.scene);
        AssetDatabase.SaveAssets();

        DebugEditor.LogAssetIntegrity($"Grid Assets Updated");
    }

    private static string GetGeneratedFilePath(GameObject sceneGrid, bool isForView)
    {
        string prefix = isForView ? "VE" : "SE";
        string sceneName = sceneGrid.scene.name;
        string fileName = $"{prefix}_Grid_{sceneName}";
        return $"{PATH}/{fileName}.prefab";
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

        LevelGridSettings GlobalGridSettings = AssetDatabase.LoadAssetAtPath<LevelGridSettings>("Assets/Config/Game/GridSettings/GridSettings.asset");
        if (GlobalGridSettings == null)
        {
            Debug.LogError("Global Grid Settings missing (GridSettings.asset), please create the file in Assets/Config/Game/GridSettings");
            return;
        }
        levelGridAuth.GlobalGridSettings = GlobalGridSettings;
    }

    private static GameObject CloneTilemap(TilemapRenderer renderer, Tilemap tilemap)
    {
        GameObject cloneGameObject = new GameObject(renderer.gameObject.name, typeof(Tilemap), typeof(TilemapRenderer));

        TilemapRenderer cloneRenderer = cloneGameObject.GetComponent<TilemapRenderer>();
        cloneRenderer.sortingLayerID = renderer.sortingLayerID;
        cloneRenderer.sortingOrder = renderer.sortingOrder;

        Tilemap cloneTilemap = cloneGameObject.GetComponent<Tilemap>();
        cloneTilemap.size = tilemap.size;
        cloneTilemap.origin = tilemap.origin;

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        cloneTilemap.SetTilesBlock(bounds, tiles);

        return cloneGameObject;
    }
}