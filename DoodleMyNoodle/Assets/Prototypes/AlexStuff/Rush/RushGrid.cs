using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushGrid : MonoBehaviour
{
    List<RushToy> toysOnGrid = new List<RushToy>();

    public void ToyHasBeenSpawned(RushToy newToy)
    {
        toysOnGrid.Add(newToy);
    }

    public void ToyHasBeenDestroyed(RushToy destroyedToy)
    {
        toysOnGrid.Remove(destroyedToy);
    }

    public void UnresolveAll()
    {
        for (int i = 0; i < toysOnGrid.Count; i++)
        {
            toysOnGrid[i].resolvedThisTurn = false;
        }
    }

    public RushToy GetToyAt(Vector2 position)
    {
        for (int i = 0; i < toysOnGrid.Count; i++)
        {
            Vector3 currentTilePos = toysOnGrid[i].transform.localPosition;
            if (currentTilePos.x == position.x && currentTilePos.y == position.y)
            {
                return toysOnGrid[i];
            }
        }
        return null;
    }
}
