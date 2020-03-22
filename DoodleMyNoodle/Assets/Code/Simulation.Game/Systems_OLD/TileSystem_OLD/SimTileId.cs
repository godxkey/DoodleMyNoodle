using System;
using System.Diagnostics;
using UnityEngine;

[NetSerializable]
[Serializable]
[DebuggerDisplay("tile({x}, {y})")] // tells visual studio how to display the variable while debugging
public struct SimTileId_OLD
{
    public int X;
    public int Y;


    public SimTileId_OLD(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    #region Positioning
    /// <summary>
    /// Returns the center of the tile
    /// </summary>
    public FixVector2 GetWorldPosition2D() => new FixVector2(X, Y);
    /// <summary>
    /// Returns the center of the tile
    /// </summary>
    public FixVector3 GetWorldPosition3D() => new FixVector3(X, Y, 0);

    public FixVector2 GetTopLeftWorldPosition2D()      => new FixVector2(X - (Fix64)0.5, Y + (Fix64)0.5);
    public FixVector2 GetTopCenterWorldPosition2D()    => new FixVector2(X             , Y + (Fix64)0.5);
    public FixVector2 GetTopRightWorldPosition2D()     => new FixVector2(X + (Fix64)0.5, Y + (Fix64)0.5);
    public FixVector2 GetMiddleLeftWorldPosition2D()   => new FixVector2(X - (Fix64)0.5, Y             );
    public FixVector2 GetMiddleRightWorldPosition2D()  => new FixVector2(X + (Fix64)0.5, Y             );
    public FixVector2 GetBottomLeftWorldPosition2D()   => new FixVector2(X - (Fix64)0.5, Y - (Fix64)0.5);
    public FixVector2 GetBottomCenterWorldPosition2D() => new FixVector2(X             , Y - (Fix64)0.5);
    public FixVector2 GetBottomRightWorldPosition2D()  => new FixVector2(X + (Fix64)0.5, Y - (Fix64)0.5);

    public FixVector3 GetTopLeftWorldPosition3D()      => new FixVector3(X - (Fix64)0.5, Y + (Fix64)0.5, 0);
    public FixVector3 GetTopCenterWorldPosition3D()    => new FixVector3(X             , Y + (Fix64)0.5, 0);
    public FixVector3 GetTopRightWorldPosition3D()     => new FixVector3(X + (Fix64)0.5, Y + (Fix64)0.5, 0);
    public FixVector3 GetMiddleLeftWorldPosition3D()   => new FixVector3(X - (Fix64)0.5, Y             , 0);
    public FixVector3 GetMiddleRightWorldPosition3D()  => new FixVector3(X + (Fix64)0.5, Y             , 0);
    public FixVector3 GetBottomLeftWorldPosition3D()   => new FixVector3(X - (Fix64)0.5, Y - (Fix64)0.5, 0);
    public FixVector3 GetBottomCenterWorldPosition3D() => new FixVector3(X             , Y - (Fix64)0.5, 0);
    public FixVector3 GetBottomRightWorldPosition3D()  => new FixVector3(X + (Fix64)0.5, Y - (Fix64)0.5, 0);
    #endregion

    // Used to calculate directions
    public static Vector2Int operator -(in SimTileId_OLD a, in SimTileId_OLD b)
    {
        return new Vector2Int(a.X - b.X, a.Y - b.Y);
    }
    // Used to calculate directions
    public static SimTileId_OLD operator +(in SimTileId_OLD a, in Vector2Int dir)
    {
        return new SimTileId_OLD(a.X + dir.x, a.Y + dir.y);
    }



    #region Builder method
    public static SimTileId_OLD FromWorldPosition(in FixVector3 worldPosition)
    {
        return new SimTileId_OLD(Fix64.RoundToInt(worldPosition.x), Fix64.RoundToInt(worldPosition.y));
    }

    #endregion

    #region Overloads
    public static bool operator ==(in SimTileId_OLD a, in SimTileId_OLD b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(in SimTileId_OLD a, in SimTileId_OLD b) => !(a == b);
    public override bool Equals(object obj)
    {
        if (!(obj is SimTileId_OLD))
        {
            return false;
        }

        var objTileId = (SimTileId_OLD)obj;
        return (X == objTileId.X) && (Y == objTileId.Y);
    }
    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + X.GetHashCode();
        hashCode = hashCode * -1521134295 + Y.GetHashCode();
        return hashCode;
    }
    #endregion
}