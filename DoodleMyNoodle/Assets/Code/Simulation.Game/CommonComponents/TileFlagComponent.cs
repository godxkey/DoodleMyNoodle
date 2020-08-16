using Unity.Entities;

[System.Flags]
public enum TileFlags : int
{
    // NB: Many of these a mutually exclusive, but we use a bitmask to help with queries

    None = 0,
    Terrain = 1 << 0,
    Ladder = 1 << 1,
    Empty = 1 << 2,
    
    All = ~0
}

public struct TileFlagComponent : IComponentData
{
    public TileFlags Value;

    public bool IsTerrain => (Value & TileFlags.Terrain) != 0;
    public bool IsLadder => (Value & TileFlags.Ladder) != 0;

    public static implicit operator TileFlags(TileFlagComponent val) => val.Value;
    public static implicit operator TileFlagComponent(TileFlags val) => new TileFlagComponent() { Value = val };
}
