using System;
using Unity.Mathematics;

/// <summary>
/// Contains helper math methods.
/// </summary>
public static class fixMath
{
    /// <summary>
    /// Approximate value of Pi.
    /// </summary>
    public static readonly fix Pi = global::fix.Pi;

    /// <summary>
    /// Approximate value of Pi multiplied by two.
    /// </summary>
    public static readonly fix TwoPi = global::fix.PiTimes2;

    /// <summary>
    /// Approximate value of Pi divided by two.
    /// </summary>
    public static readonly fix PiOver2 = global::fix.PiOver2;

    /// <summary>
    /// Approximate value of Pi divided by four.
    /// </summary>
    public static readonly fix PiOver4 = global::fix.Pi / new fix(4);

    /// <summary>
    /// Calculate remainder of of Fix64 division using same algorithm
    /// as Math.IEEERemainder
    /// </summary>
    /// <param name="dividend">Dividend</param>
    /// <param name="divisor">Divisor</param>
    /// <returns>Remainder</returns>
    public static fix IEEERemainder(in fix dividend, in fix divisor)
    {
        return dividend - (divisor * global::fix.Round(dividend / divisor));
    }

    /// <summary>
    /// Reduces the angle into a range from -Pi to Pi.
    /// </summary>
    /// <param name="angle">Angle to wrap.</param>
    /// <returns>Wrapped angle.</returns>
    public static fix WrapAngle(fix angle)
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
    public static fix Clamp(in fix value, in fix min, in fix max)
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
    public static fix Max(in fix a, in fix b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the two parameters.
    /// </summary>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <returns>Lower value of the two parameters.</returns>
    public static fix Min(in fix a, in fix b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">Degrees to convert.</param>
    /// <returns>Radians equivalent to the input degrees.</returns>
    public static fix ToRadians(in fix degrees)
    {
        return degrees * (Pi / F64.C180);
    }

    /// <summary>
    /// Converts radians to degrees.
    /// </summary>
    /// <param name="radians">Radians to convert.</param>
    /// <returns>Degrees equivalent to the input radians.</returns>
    public static fix ToDegrees(in fix radians)
    {
        return radians * (F64.C180 / Pi);
    }

    public static bool AlmostEqual(in fix3 a, in fix3 b)
    {
        return AlmostEqual(a, b, (fix)0.001);
    }
    public static bool AlmostEqual(in fix3 a, in fix3 b, in fix epsilon)
    {
        return global::fix3.DistanceSquared(a, b) < epsilon;
    }





    public static fix         fix(int value)                                       => new fix(value);
    public static fix         fix(float value)                                     => (fix)value;

    public static fix2    fix2(in fix v)                                     => new fix2(v, v);
    public static fix3    fix3(in fix v)                                     => new fix3(v, v, v);
    public static fix4    fix4(in fix v)                                     => new fix4(v, v, v, v);

    public static fix2    fix2(in fix x, in fix y)                         => new fix2(x, y);
    public static fix3    fix3(in fix x, in fix y, in fix z)             => new fix3(x, y, z);
    public static fix4    fix4(in fix x, in fix y, in fix z, in fix w) => new fix4(x, y, z, w);

    public static fix2    fix2(in int x, in int y)                             => new fix2(x, y);
    public static fix3    fix3(in int x, in int y, in int z)                   => new fix3(x, y, z);
    public static fix4    fix4(in int x, in int y, in int z, in int w)         => new fix4(x, y, z, w);

    public static fix3    fix3(in fix2 xy, in fix z)                   => new fix3(xy.x, xy.y, z);
    public static fix3    fix3(in fix x, in fix2 yz)                   => new fix3(x, yz.x, yz.y);
    public static fix3    fix3(in fix2 xy, in int z)                     => new fix3(xy.x, xy.y, z);
    public static fix3    fix3(in int x, in fix2 yz)                     => new fix3(x, yz.x, yz.y);
    public static fix3    fix3(in int2 xy, in int z)                           => new fix3(xy.x, xy.y, z);
    public static fix3    fix3(in int x, in int2 yz)                           => new fix3(x, yz.x, yz.y);


    public static fix length(in fix2 v) => v.length;
    public static fix length(in fix3 v) => v.length;
    public static fix length(in fix4 v) => v.length;
    public static fix lengthsq(in fix2 v) => v.lengthSquared;
    public static fix lengthsq(in fix3 v) => v.lengthSquared;
    public static fix lengthsq(in fix4 v) => v.lengthSquared;


    public static fix      round(in fix v)      =>      global::fix.Round(v);
    public static fix2 round(in fix2 v) => fix2(global::fix.Round(v.x), global::fix.Round(v.y));
    public static fix3 round(in fix3 v) => fix3(global::fix.Round(v.x), global::fix.Round(v.y), global::fix.Round(v.z));
    public static fix4 round(in fix4 v) => fix4(global::fix.Round(v.x), global::fix.Round(v.y), global::fix.Round(v.z), global::fix.Round(v.w));

}
