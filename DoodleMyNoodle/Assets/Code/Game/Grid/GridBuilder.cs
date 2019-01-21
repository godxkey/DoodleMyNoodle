using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder
{
    // All data that are calculated (not choosen by user, or not yet choosen depending on mode (Grid/Tile))
    private class GridCalculatedData
    {
        // From Grid info
        public float deltaX;
        public float deltaY;
        public int tileAmount;
        public Vector3 centerPoint;

        // From Tile Info
        // todo
    }

    // Setup the info we'll send to the grid
    public static void SetupGrid(GridData data, bool useTileData)
    {
        if (!Grid.hasBeenSetup)
        {
            GridCalculatedData newData = new GridCalculatedData();

            if (useTileData)
            {
                Debug.LogError("Not Supported at the Moment. Contact Alex");
            }
            else
            {
                newData.deltaX = ((Mathf.Abs(data.cornerALocation.x) + Mathf.Abs(data.cornerALocation.x)) / data.gridSize);
                newData.deltaY = ((Mathf.Abs(data.cornerBLocation.y) + Mathf.Abs(data.cornerBLocation.y)) / data.gridSize);
                newData.tileAmount = data.gridSize * data.gridSize;
                newData.centerPoint = new Vector3(data.cornerALocation.x + (newData.deltaX * (data.gridSize / 2)), data.cornerALocation.y - (newData.deltaY * (data.gridSize / 2)), 0);
            }

            // Apply to grid object
            UpdateGrid(newData,data);

        }
        else
        {
            Debug.LogWarning("There's already a grid system");
        }
    }

    // Put all data inside the Grid
    private static void UpdateGrid(GridCalculatedData calculatedData, GridData data)
    {
        Grid.centerPoint = calculatedData.centerPoint;

        Grid.cornerPosA = data.cornerALocation;
        Grid.cornerPosB = data.cornerBLocation;

        Grid.tileAmount = calculatedData.tileAmount;
        Grid.gridSize = data.gridSize;

        Grid.deltaX = calculatedData.deltaX;
        Grid.deltaY = calculatedData.deltaY;

        Grid.hasBeenSetup = true;
    }

    // This function is for debugging. You can get all positions from a potential grid created from 2 corners and some data
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
            }
            else
            {
                posX = (cornerA.x + (deltaX / 2)) + (deltaX * (column - 1));
            }

            if (row == 1)
            {
                posY = (cornerA.y - (deltaY / 2));
            }
            else
            {
                posY = (cornerA.y - (deltaY / 2)) - (deltaY * (row - 1));
            }

            previousPosX = posX;
            previousPosY = posY;

            gridPos.Add(new Vector3(posX, posY, 0));
        }
    }
}
