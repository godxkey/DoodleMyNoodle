using System;

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
}
