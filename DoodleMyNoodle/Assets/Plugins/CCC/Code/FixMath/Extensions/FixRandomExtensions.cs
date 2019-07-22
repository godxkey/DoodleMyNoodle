using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FixRandomExtensions
{
    public static Fix64 RandomRange(this FixRandom random, Fix64 min, Fix64 max)
    {
        return (random.NextFix01() * (max - min)) + min;
    }

    /// <summary>
    /// Vector will be normalized
    /// </summary>
    public static FixVector2 RandomDirection2D(this FixRandom random)
    {
        Fix64 angle = random.NextFix01() * Fix64.PiTimes2;

        return new FixVector2(
            Fix64.Cos(angle),   // x
            Fix64.Sin(angle));  // y
    }

    /// <summary>
    /// Vector will be normalized
    /// </summary>
    public static FixVector3 RandomDirection3D(this FixRandom random)
    {
        Fix64 phi = random.NextFix01() * Fix64.PiTimes2;
        Fix64 costheta = random.RandomRange(-1, 1);
        Fix64 theta = Fix64.Acos(costheta);

        return new FixVector3(
            Fix64.Sin(theta) * Fix64.Cos(phi), // x
            Fix64.Sin(theta) * Fix64.Sin(phi), // y
            Fix64.Cos(theta));                 // z
    }
}
