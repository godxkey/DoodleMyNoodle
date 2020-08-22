using Unity.Entities;
using Unity.Mathematics;

public struct GridTileReference : IBufferElementData
{
    public Entity Tile;

    public static implicit operator Entity(GridTileReference val) => val.Tile;
    public static implicit operator GridTileReference(Entity val) => new GridTileReference() { Tile = val };
}
