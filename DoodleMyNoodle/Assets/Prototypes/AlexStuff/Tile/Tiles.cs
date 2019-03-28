using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles
{
    List<BaseTile> tiles;

    public Tiles(List<BaseTile> roomTiles)
    {
        tiles = roomTiles;
    }

    public BaseTile GetTileByID(int tileID)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if(tiles[i].TileID == tileID)
            {
                return tiles[i];
            }
        }
        return null;
    }
}
