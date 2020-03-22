using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimTransformTileExtensions
{
    /// <summary>
    /// Get the closest tile that matches with the world position
    /// </summary>
    public static SimTileId_OLD GetTileId(this SimTransformComponent tr)
    {
        return SimTileId_OLD.FromWorldPosition(tr.WorldPosition);
    }
}
