﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonRoom : MonoBehaviour
{
    [SerializeField]
    private Grid grid;
    public Grid Grid { get { return grid; } }

    [SerializeField]
    private Tiles tiles;
    public Tiles Tiles { get { return tiles; } }

    public UnityEvent onGridInitialize = new UnityEvent();

    public void SetGrid(Grid grid)
    {
        this.grid = grid;
        onGridInitialize.Invoke();
    }

    public void SetTiles(Tiles tiles)
    {
        this.tiles = tiles;
    }
}
