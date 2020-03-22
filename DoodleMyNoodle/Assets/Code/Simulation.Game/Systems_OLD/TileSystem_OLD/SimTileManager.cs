using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NB: Even though this class is a SimComponent, we do not use the "SimXComponent" naming because logically, this is not a component.
/// <para/>
/// This is a data manager.
/// </summary>

public class SimTileManager : SimSingleton<SimTileManager>
{
    [SerializeField]
    int _width = 10;
    [SerializeField]
    int _height = 10;

    public bool IsTileWalkable(in SimTileId_OLD tileId)
    {
        // is rock ? is lava ? etc.

        return IsTileInBound(tileId);
    }

    public bool IsTileInBound(in SimTileId_OLD tileId)
    {
        return tileId.X >= 0
            && tileId.X < _width
            && tileId.Y >= 0
            && tileId.Y < _height;
    }

    public Plane FloorPlane => new Plane(Vector3.back, 0);
}
