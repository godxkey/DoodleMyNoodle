using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    // Grid Data

    public static Vector3 centerPoint;

    public static Vector3 cornerPosA;
    public static Vector3 cornerPosB;

    public static int tileAmount;
    public static int gridSize;

    public static float deltaX;
    public static float deltaY;

    // prevent constant updates of data
    public static bool hasBeenSetup = false;

    // Interacting and Asking for Info

    public static Vector3 GetTilePosition(TileID tileID)
    {
        if (tileID > tileAmount)
            Debug.LogWarning("TileID is out of bound");

        int column = GetColumnFromID(tileID);
        int row = GetRowFromID(tileID);

        return GetTilePosition(column, row);
    }

    private static Vector3 GetTilePosition(int columnNumber, int rowNumber)
    {
        // Calculate the position of the tile
        float posX;
        float posY;

        if (columnNumber == 1)
        {
            posX = (cornerPosA.x + (deltaX / 2));
        }
        else
        {
            posX = (cornerPosA.x + (deltaX / 2)) + (deltaX * (columnNumber - 1));
        }

        if (rowNumber == 1)
        {
            posY = (cornerPosA.y - (deltaY / 2));
        }
        else
        {
            posY = (cornerPosA.y - (deltaY / 2)) - (deltaY * (rowNumber - 1));
        }

        return new Vector3(posX, posY, 0);
    }

    private static int GetColumnFromID(int tileID)
    {
        if (tileID > tileAmount)
            Debug.LogWarning("TileID is out of bound");

        return (tileID % gridSize) + 1;
    }

    private static int GetRowFromID(int tileID)
    {
        if (tileID > tileAmount)
            Debug.LogWarning("TileID is out of bound");

        int row;
        if ((tileID + 1) > gridSize)
            row = Mathf.CeilToInt((tileID / gridSize)) + 1;
        else
            row = 1;

        return row;
    }

    public static Vector2 GetGridLocationFromID(int tileID)
    {
        return new Vector2(GetColumnFromID(tileID), GetRowFromID(tileID));
    }

    public static int GetTileNumberFromGridLocation(Vector2 gridLocation)
    {
        return Mathf.RoundToInt(gridLocation.x * gridLocation.y);
    }
}
