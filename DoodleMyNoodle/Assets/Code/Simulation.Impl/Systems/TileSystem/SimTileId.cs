using System;
using System.Diagnostics;
using UnityEngine;

[Serializable]
[DebuggerDisplay("tile({x}, {y})")] // tells visual studio how to display the variable while debugging
public struct SimTileId
{
    public int x;
    public int y;


    public SimTileId(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public FixVector2 GetWorldPosition2D()
    {
        return new FixVector2(x, y);
    }
    public FixVector3 GetWorldPosition3D()
    {
        return new FixVector3(x, y, 0);
    }

    // Used to calculate directions
    public static Vector2Int operator -(in SimTileId a, in SimTileId b)
    {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }
    // Used to calculate directions
    public static SimTileId operator +(in SimTileId a, in Vector2Int dir)
    {
        return new SimTileId(a.x + dir.x, a.y + dir.y);
    }



    #region Builder method
    public static SimTileId FromWorldPosition(in FixVector3 worldPosition)
    {
        return new SimTileId(Fix64.RoundToInt(worldPosition.x), Fix64.RoundToInt(worldPosition.y));
    }

    #endregion

    #region Overloads
    public static bool operator ==(in SimTileId a, in SimTileId b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(in SimTileId a, in SimTileId b) => !(a == b);
    public override bool Equals(object obj)
    {
        if (!(obj is SimTileId))
        {
            return false;
        }

        var objTileId = (SimTileId)obj;
        return (x == objTileId.x) && (y == objTileId.y);
    }
    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }
    #endregion
}