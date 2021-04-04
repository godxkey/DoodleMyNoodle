using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using static Unity.MathematicsX.mathX;

public static class VectorExtensions
{
    public static Vector2 SwapXAndY(this in Vector2 v)
    {
        return new Vector2(v.y, v.x);
    }

    public static Vector2 Clamped(this in Vector2 v, in Vector2 min, in Vector2 max)
    {
        return new Vector2(Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y));
    }
    public static Vector3 Clamped(this in Vector3 v, in Vector3 min, in Vector3 max)
    {
        return new Vector3(Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y),
                Mathf.Clamp(v.z, min.z, max.z));
    }

    public static Vector2 MaxLimit(this in Vector2 v, in Vector2 max)
    {
        return new Vector2(Mathf.Min(v.x, max.x), Mathf.Min(v.y, max.y));
    }
    public static Vector3 MaxLimit(this in Vector3 v, in Vector3 max)
    {
        return new Vector3(Mathf.Min(v.x, max.x), Mathf.Min(v.y, max.y), Mathf.Min(v.z, max.z));
    }
    public static Vector2 MinLimit(this in Vector2 v, in Vector2 min)
    {
        return new Vector2(Mathf.Max(v.x, min.x), Mathf.Max(v.y, min.y));
    }
    public static Vector3 MinLimit(this in Vector3 v, in Vector3 min)
    {
        return new Vector3(Mathf.Max(v.x, min.x), Mathf.Max(v.y, min.y), Mathf.Max(v.z, min.z));
    }

    public static Vector2 MaxLength(this in Vector2 v, float maximalLength)
    {
        float mag = v.magnitude;
        if (mag > maximalLength)
        {
            return v * (maximalLength / mag);
        }
        return v;
    }
    public static Vector3 MaxLength(this in Vector3 v, float maximalLength)
    {
        float mag = v.magnitude;
        if (mag > maximalLength)
        {
            return v * (maximalLength / mag);
        }
        return v;
    }
    public static Vector2 MinLength(this in Vector2 v, float minLength)
    {
        float mag = v.magnitude;
        if (mag < minLength)
        {
            return v * (minLength / mag);
        }
        return v;
    }
    public static Vector3 MinLength(this in Vector3 v, float minLength)
    {
        float mag = v.magnitude;
        if (mag < minLength)
        {
            return v * (minLength / mag);
        }
        return v;
    }

    public static Vector2 Rounded(this in Vector2 v)
    {
        return new Vector2(round(v.x), round(v.y));
    }

    public static Vector2Int RoundedToInt(this in Vector2 v)
    {
        return new Vector2Int((int)round(v.x), (int)round(v.y));
    }
    public static int2 RoundedToInt2(this in Vector2 v)
    {
        return new int2((int)round(v.x), (int)round(v.y));
    }

    public static Vector2 Rotate(this in Vector2 v, float angle)
    {
        return v.RotateRad(angle * Mathf.Deg2Rad);
    }
    public static Vector2 RotateRad(this in Vector2 v, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(
            (cos * tx) - (sin * ty),    // x
            (sin * tx) + (cos * ty));   // y
    }

    public static Vector2 FlippedX(this in Vector2 v)
    {
        return new Vector2(-v.x, v.y);
    }

    public static Vector2 FlippedY(this in Vector2 v)
    {
        return new Vector2(v.x, -v.y);
    }

    public static Vector2 MovedTowards(this in Vector2 v, in Vector2 target, float maxDistanceDelta)
    {
        return Vector2.MoveTowards(v, target, maxDistanceDelta);
    }
    public static Vector3 MovedTowards(this in Vector3 v, in Vector3 target, float maxDistanceDelta)
    {
        return Vector3.MoveTowards(v, target, maxDistanceDelta);
    }

    public static Quaternion ToEulerRotation(this in Vector3 v)
    {
        return Quaternion.Euler(v);
    }

    public static Vector2 Lerpped(this in Vector2 v, in Vector2 target, float t)
    {
        return Vector2.Lerp(v, target, t);
    }
    public static Vector3 Lerpped(this in Vector3 v, in Vector3 target, float t)
    {
        return Vector3.Lerp(v, target, t);
    }

    /// <summary>
    /// Multiplie les parametre par ceux du 'scale'
    /// </summary>
    public static Vector2 Scaled(this in Vector2 v, in Vector2 scale)
    {
        return new Vector3(
            v.x * scale.x,
            v.y * scale.y);
    }

    /// <summary>
    /// Divise les parametre par ceux du 'invertedScale'
    /// </summary>
    public static Vector2 ScaledInv(this in Vector2 v, in Vector2 invertedScale)
    {
        return new Vector3(
            v.x / invertedScale.x,
            v.y / invertedScale.y);
    }
    /// <summary>
    /// Multiplie les parametre par ceux du 'scale'
    /// </summary>
    public static Vector3 Scaled(this in Vector3 v, in Vector3 scale)
    {
        return new Vector3(
            v.x * scale.x,
            v.y * scale.y,
            v.z * scale.z);
    }

    /// <summary>
    /// Divise les parametre par ceux du 'invertedScale'
    /// </summary>
    public static Vector3 ScaledInv(this in Vector3 v, in Vector3 invertedScale)
    {
        return new Vector3(
            v.x / invertedScale.x,
            v.y / invertedScale.y,
            v.z / invertedScale.z);
    }

    public static Vector2 Abs(this in Vector2 v)
    {
        return new Vector2(abs(v.x), abs(v.y));
    }
    public static Vector3 Abs(this in Vector3 v)
    {
        return new Vector3(abs(v.x), abs(v.y), abs(v.z));
    }
}
