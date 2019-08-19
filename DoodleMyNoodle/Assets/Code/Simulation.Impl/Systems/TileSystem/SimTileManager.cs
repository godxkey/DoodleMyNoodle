using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NB: Even though this class is a SimComponent, we do not use the "SimComp" prefix because logically, this is not a component.
/// <para/>
/// This is a data manager.
/// </summary>

public class SimTileManager : SimCompSingleton<SimTileManager>
{
    [SerializeField]
    int width;
    [SerializeField]
    int height;

    public bool IsTileWalkable(in SimTileId tileId)
    {
        // TODO: is rock ? is lava ? etc.

        return IsTileInBound(tileId);
    }

    public bool IsTileInBound(in SimTileId tileId)
    {
        return tileId.x >= 0
            && tileId.x < width
            && tileId.y >= 0
            && tileId.y < height;
    }
}
