using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Create/DMN/GridData")]
public class GridData : ScriptableObject
{
    public int gridSize; // Amount of Tile

    public static int defaultGridTileSize = 16;

    public GridData(int gridTileSize)
    {
        this.gridSize = gridTileSize;
    }

    // a single grid tile objects data structure

    // add button to open level design window
    // in this window we can say what is on the tile at start
}
