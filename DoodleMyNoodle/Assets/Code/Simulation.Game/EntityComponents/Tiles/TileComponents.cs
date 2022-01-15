using System;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;

public struct TileTag : IComponentData { }

public struct TileColliderTag : IComponentData { }
public struct TileColliderReference : IComponentData
{
    public Entity ColliderEntity;
}

public struct TileId : IComponentData
{
    public short X;
    public short Y;

    private const int MIN_INT = short.MinValue + 1;
    private const int MAX_INT = short.MaxValue;
    private const short INVALID_COORD = short.MinValue;

    public TileId(int2 p) : this(p.x, p.y) { }

    public TileId(int x, int y)
    {
        if (x < MIN_INT || x > MAX_INT ||
            y < MIN_INT || y > MAX_INT)
        {
            Y = X = INVALID_COORD;
        }
        else
        {
            X = (short)x;
            Y = (short)y;
        }
    }

    public static TileId Invalid => new TileId() { X = short.MinValue, Y = short.MinValue };

    //public static implicit operator int2(TileId val) => int2(val.X, val.Y);
    //public static implicit operator TileId(int2 val) => new TileId(val);
}

[System.Flags]
public enum TileFlags : int
{
    // NB: Many of these a mutually exclusive, but we use a bitmask to help with queries

    InsideGrid = 1 << 0,
    Terrain = 1 << 1,
    Ladder = 1 << 2,
    Indestructible = 1 << 3,
    Shape_Full = 1 << 4,
    Shape_CornerTopLeft = 1 << 5,
    Shape_CornerTopRight = 1 << 6,
    Shape_CornerBottomLeft = 1 << 7,
    Shape_CornerBottomRight = 1 << 8,
    Shape_CornerAny = Shape_CornerTopLeft | Shape_CornerTopRight | Shape_CornerBottomLeft | Shape_CornerBottomRight,

    All = ~0
}

public struct TileFlagComponent : IComponentData, IEquatable<TileFlagComponent>
{
    public TileFlags Value;

    public bool IsDestructible => (Value & TileFlags.Indestructible) == 0; // has Indestructible flags
    public bool IsTerrain => (Value & TileFlags.Terrain) != 0; // has Terrain flags
    public bool IsLadder => (Value & TileFlags.Ladder) != 0; // has Ladder flags
    public bool IsEmpty => (Value & (TileFlags.Ladder | TileFlags.Terrain)) == 0; // has NO Terrain or Ladder flags
    public bool IsOutOfGrid => (Value & TileFlags.InsideGrid) == 0; // has InsideGrid

    public bool IsShapeFull => (Value & TileFlags.Shape_Full) != 0;
    public bool IsShapeCornerTopLeft => (Value & TileFlags.Shape_CornerTopLeft) != 0;
    public bool IsShapeCornerTopRight => (Value & TileFlags.Shape_CornerTopRight) != 0;
    public bool IsShapeCornerBottomLeft => (Value & TileFlags.Shape_CornerBottomLeft) != 0;
    public bool IsShapeCornerBottomRight => (Value & TileFlags.Shape_CornerBottomRight) != 0;
    public bool IsShapeCornerAny => (Value & TileFlags.Shape_CornerAny) != 0;

    public static implicit operator TileFlags(TileFlagComponent val) => val.Value;
    public static implicit operator TileFlagComponent(TileFlags val) => new TileFlagComponent() { Value = val };

    public static bool operator ==(TileFlagComponent left, TileFlagComponent right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TileFlagComponent left, TileFlagComponent right)
    {
        return !(left == right);
    }

    public static TileFlagComponent OutOfGrid => default;
    public static TileFlagComponent Empty => TileFlags.InsideGrid;
    public static TileFlagComponent Terrain => TileFlags.InsideGrid | TileFlags.Terrain | TileFlags.Shape_Full;
    public static TileFlagComponent Ladder => TileFlags.InsideGrid | TileFlags.Ladder | TileFlags.Shape_Full;
    public static TileFlagComponent Bedrock => TileFlags.InsideGrid | TileFlags.Terrain | TileFlags.Indestructible | TileFlags.Shape_Full;

    public override bool Equals(object obj)
    {
        return obj is TileFlagComponent component && Equals(component);
    }

    public bool Equals(TileFlagComponent other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
