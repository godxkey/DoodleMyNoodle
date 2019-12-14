﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NB: Even though this class is a SimComponent, we do not use the "SimComp" prefix because logically, this is not a component.
/// <para/>
/// This is a data manager.
/// </summary>

public class SimTileManager : SimSingleton<SimTileManager>
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
        return tileId.X >= 0
            && tileId.X < width
            && tileId.Y >= 0
            && tileId.Y < height;
    }
}