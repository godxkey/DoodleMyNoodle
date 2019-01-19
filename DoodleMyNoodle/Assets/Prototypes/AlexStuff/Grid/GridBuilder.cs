using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder
{
    public static void GetAllPositions(ref List<Vector3> gridPos, GridData data, Vector3 cornerA, Vector3 cornerB)
    {
        float previousPosX = cornerA.x;
        float previousPosY = cornerA.y;

        float deltaX = ((Mathf.Abs(cornerA.x) + Mathf.Abs(cornerB.x)) / data.gridSize);
        float deltaY = ((Mathf.Abs(cornerA.y) + Mathf.Abs(cornerB.y)) / data.gridSize);

        // For each tile
        for (int i = 0; i < (data.gridSize * data.gridSize); i++)
        {
            // What column number and row number are we (prevent having 2 for inside of each other)
            int column = (i % data.gridSize) + 1;
            int row;
            if ((i + 1) > data.gridSize)
                row = Mathf.CeilToInt((i / data.gridSize)) + 1;
            else
                row = 1;

            // Calculate the position of the tile
            float posX;
            float posY;

            if (column == 1)
            {
                posX = (cornerA.x + (deltaX / 2));
            } else
            {
                posX = (previousPosX + deltaX);
            }
            
            if(row == 1)
            {
                posY = (cornerA.y - (deltaY / 2));
            } else
            {
                posY = (cornerA.y - (deltaY / 2)) - (deltaY * (row - 1));
            }

            previousPosX = posX;
            previousPosY = posY;

            gridPos.Add(new Vector3(posX, posY, 0));
        }
    }


    public static void BuildTiles(ref Grid grid, GridData data, Vector3 cornerA, Vector3 cornerB)
    {
        // Stock the number of tiles in the grid
        grid.tileAmount = data.gridSize * data.gridSize;

        // Tiles must be squares
        if((Mathf.Abs(cornerA.x) + Mathf.Abs(cornerB.x)) != (Mathf.Abs(cornerA.y) + Mathf.Abs(cornerB.y)))
        {
            Debug.LogWarning("Grid Tiles won't be squares because your 2 corners aren't correctly place (distance between their x and y must be equal)");
        }

        if(cornerA.z != 0 || cornerB.z != 0)
        {
            Debug.LogError("Grid should be at 0. Move the corners");
        }

        // Calculate the size of the tiles
        grid.tileSize = (Mathf.Abs(cornerA.x) + Mathf.Abs(cornerB.x)) / data.gridSize;

        // Stock the gridSize
        grid.gridSize = data.gridSize;

        // For each tile
        for (int i = 0; i < grid.tileAmount; i++)
        {
            // What column number and row number are we (prevent having 2 for inside of each other)
            int column = i % data.gridSize;
            int row = i / data.gridSize;

            // Calculate the position of the tile
            float posX = cornerA.x + (grid.tileSize * column);
            float posY = cornerA.y - (grid.tileSize * row);

            // Create tile
            Tile currentTile = new Tile(new Vector2(posX, posY));

            // Add tile to Grid data structure
            grid.tiles[column][row] = currentTile;
        }

        SetupNeighborhood(ref grid);
    }

    static void SetupNeighborhood(ref Grid grid)
    {
        // For each tile
        for (int i = 0; i < grid.tileAmount; i++)
        {
            // What column number and row number are we
            int column = i % grid.gridSize;
            int row = i / grid.gridSize;

            // What column number and row number of neighbors

            // Neighbors 1
            int column1;
            int row1;
            Tile tile1;

            if(column != 1)
            {
                column1 = column - 1;
                row1 = row;
                tile1 = grid.tiles[column1][row1];
            } else
            {
                tile1 = null;
            }

            // Neighbors 2
            int column2;
            int row2;
            Tile tile2;

            if (column != grid.gridSize)
            {
                column2 = column + 1;
                row2 = row;
                tile2 = grid.tiles[column2][row2];
            }
            else
            {
                tile2 = null;
            }

            // Neighbors 3
            int column3;
            int row3;
            Tile tile3;

            if (row != 1)
            {
                column3 = column;
                row3 = row - 1;
                tile3 = grid.tiles[column3][row3];
            }
            else
            {
                tile3 = null;
            }

            // Neighbors 4
            int column4;
            int row4;
            Tile tile4;

            if (row != grid.gridSize)
            {
                column4 = column;
                row4 = row + 1;
                tile4 = grid.tiles[column4][row4];
            }
            else
            {
                tile4 = null;
            }

            // Add all Neighbors
            grid.tiles[column][row].SetNeighbors(tile1,tile2,tile3,tile4);
        }
    }
}
