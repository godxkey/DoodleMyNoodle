
using System.Collections.Generic;
using UnityEngine;

public class SimPathService : SimCompSingleton<SimPathService>
{
    // fbesette: should use pooling for list of tiles
    public bool GetPathTo(SimGridWalkerComponent walker, in SimTileId destination, ref List<SimTileId> result)
    {
        // The algo here is pretty dumb. It's not a real pathfinder. We should implement one eventually
        if (result == null)
            result = new List<SimTileId>();

        result.Clear();
        List<SimTileId> resultRef = result;

        SimTileId currentTile = walker.tileId;

        while (currentTile != destination)
        {
            
            bool TryAdjacentTile(in SimTileId tile) // this is a method
            {
                if (SimHelper.CanEntityWalkGoOntoTile(walker, tile))
                {
                    resultRef.Add(tile);
                    currentTile = tile;
                    return true;
                }
                else
                {
                    return false;
                }
            }


            Vector2Int delta = destination - currentTile;

            // try every direction
            if (delta.x > 0 && TryAdjacentTile(currentTile + Vector2Int.right))
            {
                continue;
            }
            else if (delta.x < 0 && TryAdjacentTile(currentTile + Vector2Int.left))
            {
                continue;
            }
            else if (delta.y > 0 && TryAdjacentTile(currentTile + Vector2Int.up))
            {
                continue;
            }
            else if (delta.y < 0 && TryAdjacentTile(currentTile + Vector2Int.down))
            {
                continue;
            }

            return false; // failed at every direction!
        }

        return true;
    }
}