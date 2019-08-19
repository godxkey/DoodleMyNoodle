using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCompGridTransform : SimTransform
{
    public SimTileId tileId => SimTileId.FromWorldPosition(worldPosition);
}
