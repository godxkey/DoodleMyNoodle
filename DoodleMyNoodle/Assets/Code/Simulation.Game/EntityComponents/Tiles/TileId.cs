using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;

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