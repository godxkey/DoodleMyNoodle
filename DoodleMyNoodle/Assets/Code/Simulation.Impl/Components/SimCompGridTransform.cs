using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCompGridTransform : SimTransform
{
    public Vector2Int tile => new Vector2Int(Fix64.RoundToInt(worldPosition.X), Fix64.RoundToInt(worldPosition.Y));
}
