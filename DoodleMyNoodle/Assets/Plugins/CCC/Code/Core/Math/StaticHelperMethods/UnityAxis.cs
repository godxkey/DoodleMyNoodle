using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial struct CMath
{
    /// <summary>
    /// 0 degrees -> World2D.right
    /// <para/>
    /// 90 degrees -> World2D.up
    /// <para/>
    /// 180 degrees -> World2D.left
    /// <para/>
    /// 270 degrees -> World2D.down
    /// </summary>
    public static Vector2 AngleToVector2D(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
    /// <summary>
    /// 0 degrees -> World.forward
    /// <para/>
    /// 90 degrees -> World.down
    /// <para/>
    /// 180 degrees -> World.back
    /// <para/>
    /// 270 degrees -> World.up
    /// </summary>
    public static Vector3 AngleToVectorXPlane(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(0, -Mathf.Sin(rad), Mathf.Cos(rad));
    }
    /// <summary>
    /// 0 degrees -> World.right
    /// <para/>
    /// 90 degrees -> World.back
    /// <para/>
    /// 180 degrees -> World.left
    /// <para/>
    /// 270 degrees -> World.forward
    /// </summary>
    public static Vector3 AngleToVectorYPlane(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
    }
    /// <summary>
    /// 0 degrees -> World.up
    /// <para/>
    /// 90 degrees -> World.left
    /// <para/>
    /// 180 degrees -> World.down
    /// <para/>
    /// 270 degrees -> World.right
    /// </summary>
    public static Vector3 AngleToVectorZPlane(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0);
    }

    /// <summary>
    /// World.right -> 0 degrees
    /// <para/>
    /// World.up -> 90 degrees
    /// <para/>
    /// World.left -> 180 degrees
    /// <para/>
    /// World.down -> 270 degrees
    /// </summary>
    public static float VectorToAngle2D(Vector2 v)
    {
        if (v.x < 0)
            return Mathf.Atan(v.y / v.x) * Mathf.Rad2Deg + 180;
        return Mathf.Atan(v.y / v.x) * Mathf.Rad2Deg;
    }
    /// <summary>
    /// World.forward -> 0 degrees
    /// <para/>
    /// World.down -> 90 degrees
    /// <para/>
    /// World.back -> 180 degrees
    /// <para/>
    /// World.up -> 270 degrees
    /// </summary>
    public static float VectorToAngleXPlane(Vector3 v)
    {
        v.y = -v.y;
        if (v.z < 0)
            return Mathf.Atan(v.y / v.z) * Mathf.Rad2Deg + 180;
        return Mathf.Atan(v.y / v.z) * Mathf.Rad2Deg;
    }
    /// <summary>
    /// World.right -> 0 degrees
    /// <para/>
    /// World.back -> 90 degrees
    /// <para/>
    /// World.left -> 180 degrees
    /// <para/>
    /// World.forward -> 270 degrees
    /// </summary>
    public static float VectorToAngleYPlane(Vector3 v)
    {
        v.z = -v.z;
        if (v.x < 0)
            return Mathf.Atan(v.z / v.x) * Mathf.Rad2Deg + 180;
        return Mathf.Atan(v.z / v.x) * Mathf.Rad2Deg;
    }
    /// <summary>
    /// World.up -> 0 degrees
    /// <para/>
    /// World.left -> 90 degrees
    /// <para/>
    /// World.down -> 180 degrees
    /// <para/>
    /// World.right -> 270 degrees
    /// </summary>
    public static float VectorToAngleZPlane(Vector3 v)
    {
        return Mathf.Atan(v.x / v.y) * Mathf.Rad2Deg;
    }
}
