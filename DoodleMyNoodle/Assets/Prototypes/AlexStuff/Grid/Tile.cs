using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector2 position;

    public List<Tile> neighbors;

    public Tile(Vector2 position)
    {
        this.position = position;
    }

    public void SetNeighbors(Tile voisin1, Tile voisin2, Tile voisin3, Tile voisin4)
    {
        neighbors = new List<Tile>();
        neighbors.Add(voisin1);
        neighbors.Add(voisin2);
        neighbors.Add(voisin3);
        neighbors.Add(voisin4);
    }
}
