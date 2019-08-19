using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class DungeonTiles : MonoBehaviour
{
    [SerializeField]
    private DungeonRoom room;
    public DungeonRoom Room { get { return room; } }

    [SerializeField]
    private DungeonGrid dungeonGrid;
    public DungeonGrid DungeonGrid { get { return dungeonGrid; } }

    [System.Serializable]
    public class Row
    {
        [SerializeField]
        public bool[] rowData;
        public Row(int size) { rowData = new bool[size]; }
    }
    [SerializeField,HideInInspector]
    private Row[] inspectorData;
    public Row[] InspectorData { get { return inspectorData; } }

    private bool[,] isBlockedMatrix;
    public bool[,] IsBlockedMatrix { get { return isBlockedMatrix; } }

    [SerializeField,Header("Level Editor")]
    private BaseTile currentTilePrefab;
    public BaseTile CurrentTilePrefab { get { return currentTilePrefab; } }

    void Start()
    {
        InitializeBlockingMatric();

        room.onGridInitialize.AddListener(ConfigurationOfTilesInRoom);
    }

    public void ConfigurationOfTilesInRoom()
    {
        room.SetTiles(SpawnTiles());
    }

    private void InitializeBlockingMatric()
    {
        isBlockedMatrix = new bool[inspectorData.Length, inspectorData.Length];
        FillBockingMatrix();
    }

    private void FillBockingMatrix()
    {
        for (int i = 0; i < inspectorData.Length; i++)
        {
            for (int j = 0; j < inspectorData[i].rowData.Length; j++)
            {
                isBlockedMatrix[i, j] = inspectorData[i].rowData[j];
            }
        }
    }

    private Tiles SpawnTiles()
    {
        List<BaseTile> newTiles = new List<BaseTile>();

        int currentIndex = 0;
        foreach (bool tileIsBlocking in isBlockedMatrix)
        {
            if (tileIsBlocking)
            {
                BlockingTile newBlockingTile = Instantiate(currentTilePrefab) as BlockingTile;
                newBlockingTile.transform.SetPositionAndRotation(room.Grid.GetTilePosition(currentIndex), Quaternion.identity);
                newBlockingTile.SetTileID(currentIndex);
                newTiles.Add(newBlockingTile);
            }
            currentIndex++;
        }

        return new Tiles(newTiles);
    }

    public void InitializInspectorData(int size)
    {
        inspectorData = new Row[size];
        for (int i = 0; i < inspectorData.Length; i++)
        {
            inspectorData[i] = new Row(size);
        }
    }

    public void SetMatrixInspectorData(int column, int row, bool data)
    {
        inspectorData[column].rowData[row] = data;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DungeonTiles))]
public class DungeonTilesEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        DungeonTiles tiles = target as DungeonTiles;

        GridAlex grid = new GridAlex();
        tiles.DungeonGrid.Data.SetGridValues(ref grid);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create/Reset Tile Grid"))
        {
            tiles.InitializInspectorData(grid.gridSize);
            Save(tiles);
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < grid.gridSize; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < grid.gridSize; j++)
            {
                if (tiles.InspectorData != null)
                {
                    string buttonText = "";
                    if (tiles.InspectorData[i].rowData[j])
                    {
                        GUI.color = tiles.CurrentTilePrefab.GetTileEditorColor();
                        buttonText = tiles.CurrentTilePrefab.GetTileEditorName();
                    }

                    if (GUILayout.Button(buttonText))
                    {
                        tiles.SetMatrixInspectorData(i, j, !tiles.InspectorData[i].rowData[j]);
                        Save(tiles);
                    }

                    GUI.color = Color.white;
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    private void Save(DungeonTiles tiles)
    {
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(tiles);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }
}
#endif
