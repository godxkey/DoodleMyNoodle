using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder
{
    public GridBuilder(ref Grid grid, GridData data, Vector3 cornerA, Vector3 cornerB)
    {
        // Stock the number of tiles in the grid
        grid.tileAmount = data.gridSize * data.gridSize;

        // Tiles must be squares
        if((cornerA.x + cornerB.x) != (cornerA.y + cornerB.y))
        {
            Debug.LogWarning("Grid Tiles won't be squares because your 2 corners aren't correctly place (distance between their x and y must be equal)");
        }

        // Calculate the size of the tiles
        grid.tileSize = (cornerA.x + cornerB.x) / data.gridSize;

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

    void SetupNeighborhood(ref Grid grid)
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
