using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAlex // J'ai du renommé la class parce que Unity a déjà une class qui s'appelle Grid
{
    // Grid Data

    public Vector3 centerPoint;

    public Vector3 cornerPosA;
    public Vector3 cornerPosB;

    public int tileAmount;
    public int gridSize;

    public float tileSizeX;
    public float tileSizeY;

    // prevent constant updates of data
    public bool hasBeenSetup = false;

    public GridAlex(){}

    // Interacting and Asking for Info

    public Vector3 GetTilePosition(int tileID)
    {
        if (tileID > tileAmount)
            Debug.LogWarning("TileID is out of bound");

        int column = GetColumnFromID(tileID);
        int row = GetRowFromID(tileID);

        return GetTilePosition(column, row);
    }

    private Vector3 GetTilePosition(int columnNumber, int rowNumber)
    {
        // Calculate the position of the tile
        float posX;
        float posY;

        if (columnNumber == 1)
        {
            posX = (cornerPosA.x + (tileSizeX / 2));
        }
        else
        {
            posX = (cornerPosA.x + (tileSizeX / 2)) + (tileSizeX * (columnNumber - 1));
        }

        if (rowNumber == 1)
        {
            posY = (cornerPosA.y - (tileSizeY / 2));
        }
        else
        {
            posY = (cornerPosA.y - (tileSizeY / 2)) - (tileSizeY * (rowNumber - 1));
        }

        return new Vector3(posX, posY, 0);
    }

    private int GetColumnFromID(int tileID)
    {
        if (tileID > tileAmount)
            Debug.LogWarning("TileID is out of bound");

        return (tileID % gridSize) + 1;
    }

    private int GetRowFromID(int tileID)
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

    public Vector2 GetGridLocationFromID(int tileID)
    {
        return new Vector2(GetColumnFromID(tileID), GetRowFromID(tileID));
    }
}
