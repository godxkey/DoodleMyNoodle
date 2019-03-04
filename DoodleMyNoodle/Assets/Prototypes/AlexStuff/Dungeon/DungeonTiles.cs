using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
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
}

[CustomEditor(typeof(DungeonTiles))]
public class DungeonTilesEditor : Editor
{

    void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        DungeonTiles tiles = target as DungeonTiles;

        Grid grid = new Grid();
        tiles.DungeonGrid.Data.SetGridValues(ref grid);

        for (int i = 0; i < grid.gridSize; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < grid.gridSize; j++)
            {
                if (GUILayout.Button(""))
                {

                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
