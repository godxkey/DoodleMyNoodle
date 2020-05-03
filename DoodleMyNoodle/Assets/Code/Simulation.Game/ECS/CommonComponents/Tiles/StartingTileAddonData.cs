using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct StartingTileAddonData : IBufferElementData
{
    public Entity Prefab;
    public fix2 Position;
}

public struct GridInfoContainer : IComponentData
{
    public int GridSize;
}
