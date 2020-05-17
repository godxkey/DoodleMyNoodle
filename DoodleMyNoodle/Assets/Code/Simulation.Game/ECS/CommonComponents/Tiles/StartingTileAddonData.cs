using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.MathematicsX;

public struct StartingTileAddonData : IBufferElementData
{
    public Entity Prefab;
    public fix2 Position;
}

public struct GridInfo : IComponentData
{
    public intRect GridRect;
}
