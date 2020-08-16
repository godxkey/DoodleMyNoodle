using Unity.Entities;

[System.Flags]
public enum TileFlags : int
{
    Empty = 0,
    Terrain = 1 << 0,
    Ladder = 1 << 1,
    Character = 1 << 2,
}

public struct TileFlagComponent : IComponentData
{
    public TileFlags Value;

    public static implicit operator TileFlags(TileFlagComponent val) => val.Value;
    public static implicit operator TileFlagComponent(TileFlags val) => new TileFlagComponent() { Value = val };
}
