using UnityEngine;

public static class FixVectorExtensions
{
    public static Vector2 ToUnityVec(this in FixVector2 fixVec)
    {
        return new Vector2((float)fixVec.x, (float)fixVec.y);
    }
    public static Vector3 ToUnityVec(this in FixVector3 fixVec)
    {
        return new Vector3((float)fixVec.x, (float)fixVec.y, (float)fixVec.z);
    }
    public static Vector4 ToUnityVec(this in FixVector4 fixVec)
    {
        return new Vector4((float)fixVec.x, (float)fixVec.y, (float)fixVec.z, (float)fixVec.w);
    }


    /// <summary>
    /// Uses MaxLimit or MinLimit depending on the 'max' vector direction
    /// </summary>
    public static FixVector2 LimitDirection(this in FixVector2 v, in FixVector2 max)
    {
        Fix64 x;
        Fix64 y;

        x = (max.x > 0) ? FixMath.Min(max.x, v.x) : FixMath.Max(max.x, v.x);
        y = (max.y > 0) ? FixMath.Min(max.y, v.y) : FixMath.Max(max.y, v.y);

        return new FixVector2(x, y);
    }
    /// <summary>
    /// Uses MaxLimit or MinLimit depending on the 'max' vector direction
    /// </summary>
    public static FixVector3 LimitDirection(this in FixVector3 v, in FixVector3 max)
    {
        Fix64 x;
        Fix64 y;
        Fix64 z;

        x = (max.x > 0) ? FixMath.Min(max.x, v.x) : FixMath.Max(max.x, v.x);
        y = (max.y > 0) ? FixMath.Min(max.y, v.y) : FixMath.Max(max.y, v.y);
        z = (max.z > 0) ? FixMath.Min(max.z, v.z) : FixMath.Max(max.z, v.z);

        return new FixVector3(x, y, z);
    }

    /// <summary>
    /// Return a vector where each component is lower or equal to its equivalent in 'max'
    /// </summary>
    public static FixVector2 MaxLimit(this in FixVector2 v, in FixVector2 max)
    {
        return new FixVector2(FixMath.Min(v.x, max.x), FixMath.Min(v.y, max.y));
    }
    /// <summary>
    /// Return a vector where each component is lower or equal to its equivalent in 'max'
    /// </summary>
    public static FixVector3 MaxLimit(this in FixVector3 v, in FixVector3 max)
    {
        return new FixVector3(FixMath.Min(v.x, max.x), FixMath.Min(v.y, max.y), FixMath.Min(v.z, max.z));
    }

    /// <summary>
    /// Return a vector where each component is higher or equal to its equivalent in 'max'
    /// </summary>
    public static FixVector2 MinLimit(this in FixVector2 v, in FixVector2 min)
    {
        return new FixVector2(FixMath.Max(v.x, min.x), FixMath.Max(v.y, min.y));
    }
    /// <summary>
    /// Return a vector where each component is higher or equal to its equivalent in 'max'
    /// </summary>
    public static FixVector3 MinLimit(this in FixVector3 v, in FixVector3 min)
    {
        return new FixVector3(FixMath.Max(v.x, min.x), FixMath.Max(v.y, min.y), FixMath.Max(v.z, min.z));
    }
}