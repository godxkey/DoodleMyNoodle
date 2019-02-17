using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class not use at the moment...
public class TileID
{
    // Data

    public int tileNumber;
    public Vector2 gridLocation;

    // Constructors

    TileID(int tileNumber)
    {
        this.tileNumber = tileNumber;
        //gridLocation = Grid.GetGridLocationFromID(tileNumber); 
    }

    TileID(Vector2 gridLocation)
    {
        //tileNumber = Grid.GetTileNumberFromGridLocation(gridLocation);
        this.gridLocation = gridLocation;
    }

    // Operator Behaviors

    public static implicit operator TileID(int tileNumber)
    {
        TileID output = new TileID(tileNumber);
        return tileNumber;
    }

    public static implicit operator TileID(Vector2 gridLocation)
    {
        TileID output = new TileID(gridLocation);
        return output;
    }

    public static implicit operator int (TileID input)
    {
        return input.tileNumber;
    }

    public static implicit operator Vector2 (TileID input)
    {
        return input.gridLocation;
    }
}
