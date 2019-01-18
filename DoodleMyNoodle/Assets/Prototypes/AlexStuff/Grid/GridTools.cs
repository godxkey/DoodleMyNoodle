using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTools
{
    Grid grid;

    public GridTools(Grid grid)
    {
        this.grid = grid;
    }

    public List<Vector3> GetAllTileLocation()
    {
        List<Vector3> result = new List<Vector3>();
        for (int i = 0; i < grid.tileAmount; i++)
        {
            result.Add(grid.tiles[GetColumnNumber(i)][GetRowNumber(i)].position);
        }
        return result;
    }

    int GetColumnNumber(int currentTileNumber)
    {
        return currentTileNumber % grid.gridSize;
    }

    int GetRowNumber(int currentTileNumber)
    {
        return currentTileNumber / grid.gridSize;
    }
}
