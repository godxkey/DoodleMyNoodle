using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    [SerializeField]
    private Grid grid;
    public Grid Grid { get { return grid; } }

    [SerializeField]
    private Tiles tiles;
    public Tiles Tiles { get { return tiles; } }

    public void SetGrid(Grid grid)
    {
        this.grid = grid;
    }

    public void SetTiles(Tiles tiles)
    {
        this.tiles = tiles;
    }
}
