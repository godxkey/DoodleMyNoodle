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
    public fix2 GetWorldPosition2D() => new fix2(X, Y);
    /// <summary>
    /// Returns the center of the tile
    /// </summary>
    public fix3 GetWorldPosition3D() => new fix3(X, Y, 0);

    public fix2 GetTopLeftWorldPosition2D()      => new fix2(X - (fix)0.5, Y + (fix)0.5);
    public fix2 GetTopCenterWorldPosition2D()    => new fix2(X             , Y + (fix)0.5);
    public fix2 GetTopRightWorldPosition2D()     => new fix2(X + (fix)0.5, Y + (fix)0.5);
    public fix2 GetMiddleLeftWorldPosition2D()   => new fix2(X - (fix)0.5, Y             );
    public fix2 GetMiddleRightWorldPosition2D()  => new fix2(X + (fix)0.5, Y             );
    public fix2 GetBottomLeftWorldPosition2D()   => new fix2(X - (fix)0.5, Y - (fix)0.5);
    public fix2 GetBottomCenterWorldPosition2D() => new fix2(X             , Y - (fix)0.5);
    public fix2 GetBottomRightWorldPosition2D()  => new fix2(X + (fix)0.5, Y - (fix)0.5);

    public fix3 GetTopLeftWorldPosition3D()      => new fix3(X - (fix)0.5, Y + (fix)0.5, 0);
    public fix3 GetTopCenterWorldPosition3D()    => new fix3(X             , Y + (fix)0.5, 0);
    public fix3 GetTopRightWorldPosition3D()     => new fix3(X + (fix)0.5, Y + (fix)0.5, 0);
    public fix3 GetMiddleLeftWorldPosition3D()   => new fix3(X - (fix)0.5, Y             , 0);
    public fix3 GetMiddleRightWorldPosition3D()  => new fix3(X + (fix)0.5, Y             , 0);
    public fix3 GetBottomLeftWorldPosition3D()   => new fix3(X - (fix)0.5, Y - (fix)0.5, 0);
    public fix3 GetBottomCenterWorldPosition3D() => new fix3(X             , Y - (fix)0.5, 0);
    public fix3 GetBottomRightWorldPosition3D()  => new fix3(X + (fix)0.5, Y - (fix)0.5, 0);
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
    public static SimTileId_OLD FromWorldPosition(in fix3 worldPosition)
    {
        return new SimTileId_OLD(fix.RoundToInt(worldPosition.x), fix.RoundToInt(worldPosition.y));
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