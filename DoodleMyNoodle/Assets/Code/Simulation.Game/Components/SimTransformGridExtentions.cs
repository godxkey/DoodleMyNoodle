using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimTransformGridExtensions
{
    /// <summary>
    /// Get the closest tile that matches with the world position
    /// </summary>
    public static SimTileId GetTileId(this SimTransformComponent tr)
    {
        return SimTileId.FromWorldPosition(tr.WorldPosition);
    }
}
