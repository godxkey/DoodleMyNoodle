using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct GameActionSettingTransformTile : IComponentData
{
    public TileFlags NewTileFlags;
    public fix Radius;
}