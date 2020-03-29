using System;
using Unity.Mathematics;

/// <summary>
/// Contains helper math methods.
/// </summary>
public static class FixMath
{
    /// <summary>
    /// Approximate value of Pi.
    /// </summary>
    public static readonly Fix64 Pi = Fix64.Pi;

    /// <summary>
    /// Approximate value of Pi multiplied by two.
    /// </summary>
    public static readonly Fix64 TwoPi = Fix64.PiTimes2;

    /// <summary>
    /// Approximate value of Pi divided by two.
    /// </summary>
    public static readonly Fix64 PiOver2 = Fix64.PiOver2;

    /// <summary>
    /// Approximate value of Pi divided by four.
    /// </summary>
    public static readonly Fix64 PiOver4 = Fix64.Pi / new Fix64(4);

    /// <summary>
    /// Calculate remainder of of Fix64 division using same algorithm
    /// as Math.IEEERemainder
    /// </summary>
    /// <param name="dividend">Dividend</param>
    /// <param name="divisor">Divisor</param>
    /// <returns>Remainder</returns>
    public static Fix64 IEEERemainder(in Fix64 dividend, in Fix64 divisor)
    {
        return dividend - (divisor * Fix64.Round(dividend / divisor));
    }

    /// <summary>
    /// Reduces the angle into a range from -Pi to Pi.
    /// </summary>
    /// <param name="angle">Angle to wrap.</param>
    /// <returns>Wrapped angle.</returns>
    public static Fix64 WrapAngle(Fix64 angle)
    {
        angle = IEEERemainder(angle, TwoPi);
        if (angle < -Pi)
        {
            angle += TwoPi;
            return angle;
        }
        if (angle >= Pi)
        {
            angle -= TwoPi;
        }
        return angle;

    }

    /// <summary>
    /// Clamps a value between a minimum and maximum value.
    /// </summary>
    /// <param name="value">Value to clamp.</param>
    /// <param name="min">Minimum value.  If the value is less than this, the minimum is returned instead.</param>
    /// <param name="max">Maximum value.  If the value is more than this, the maximum is returned instead.</param>
    /// <returns>Clamped value.</returns>
    public static Fix64 Clamp(in Fix64 value, in Fix64 min, in Fix64 max)
    {
        if (value < min)
            return min;
        else if (value > max)
            return max;
        return value;
    }

    /// <summary>
    /// Returns the higher value of the two parameters.
    /// </summary>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <returns>Higher value of the two parameters.</returns>
    public static Fix64 Max(in Fix64 a, in Fix64 b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the two parameters.
    /// </summary>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <returns>Lower value of the two parameters.</returns>
    public static Fix64 Min(in Fix64 a, in Fix64 b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">Degrees to convert.</param>
    /// <returns>Radians equivalent to the input degrees.</returns>
    public static Fix64 ToRadians(in Fix64 degrees)
    {
        return degrees * (Pi / F64.C180);
    }

    /// <summary>
    /// Converts radians to degrees.
    /// </summary>
    /// <param name="radians">Radians to convert.</param>
    /// <returns>Degrees equivalent to the input radians.</returns>
    public static Fix64 ToDegrees(in Fix64 radians)
    {
        return radians * (F64.C180 / Pi);
    }

    public static bool AlmostEqual(in FixVector3 a, in FixVector3 b)
    {
        return AlmostEqual(a, b, (Fix64)0.001);
    }
    public static bool AlmostEqual(in FixVector3 a, in FixVector3 b, in Fix64 epsilon)
    {
        return FixVector3.DistanceSquared(a, b) < epsilon;
    }





    public static Fix64         fix(int value)                                       => new Fix64(value);
    public static Fix64         fix(float value)                                     => (Fix64)value;

    public static FixVector2    fix2(in Fix64 v)                                     => new FixVector2(v, v);
    public static FixVector3    fix3(in Fix64 v)                                     => new FixVector3(v, v, v);
    public static FixVector4    fix4(in Fix64 v)                                     => new FixVector4(v, v, v, v);

    public static FixVector2    fix2(in Fix64 x, in Fix64 y)                         => new FixVector2(x, y);
    public static FixVector3    fix3(in Fix64 x, in Fix64 y, in Fix64 z)             => new FixVector3(x, y, z);
    public static FixVector4    fix4(in Fix64 x, in Fix64 y, in Fix64 z, in Fix64 w) => new FixVector4(x, y, z, w);

    public static FixVector2    fix2(in int x, in int y)                             => new FixVector2(x, y);
    public static FixVector3    fix3(in int x, in int y, in int z)                   => new FixVector3(x, y, z);
    public static FixVector4    fix4(in int x, in int y, in int z, in int w)         => new FixVector4(x, y, z, w);

    public static FixVector3    fix3(in FixVector2 xy, in Fix64 z)                   => new FixVector3(xy.x, xy.y, z);
    public static FixVector3    fix3(in Fix64 x, in FixVector2 yz)                   => new FixVector3(x, yz.x, yz.y);
    public static FixVector3    fix3(in FixVector2 xy, in int z)                     => new FixVector3(xy.x, xy.y, z);
    public static FixVector3    fix3(in int x, in FixVector2 yz)                     => new FixVector3(x, yz.x, yz.y);
    public static FixVector3    fix3(in int2 xy, in int z)                           => new FixVector3(xy.x, xy.y, z);
    public static FixVector3    fix3(in int x, in int2 yz)                           => new FixVector3(x, yz.x, yz.y);


    public static Fix64 length(in FixVector2 v) => v.length;
    public static Fix64 length(in FixVector3 v) => v.length;
    public static Fix64 length(in FixVector4 v) => v.length;
    public static Fix64 lengthsq(in FixVector2 v) => v.lengthSquared;
    public static Fix64 lengthsq(in FixVector3 v) => v.lengthSquared;
    public static Fix64 lengthsq(in FixVector4 v) => v.lengthSquared;


    public static Fix64      round(in Fix64 v)      =>      Fix64.Round(v);
    public static FixVector2 round(in FixVector2 v) => fix2(Fix64.Round(v.x), Fix64.Round(v.y));
    public static FixVector3 round(in FixVector3 v) => fix3(Fix64.Round(v.x), Fix64.Round(v.y), Fix64.Round(v.z));
    public static FixVector4 round(in FixVector4 v) => fix4(Fix64.Round(v.x), Fix64.Round(v.y), Fix64.Round(v.z), Fix64.Round(v.w));

}
