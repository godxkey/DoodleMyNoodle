using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class FixRandomExtensions
{
    public static bool2 NextBool2(this FixRandom random)                                           => new bool2(random.NextBool(), random.NextBool());
    public static bool3 NextBool3(this FixRandom random)                                           => new bool3(random.NextBool(), random.NextBool(), random.NextBool());
    public static bool4 NextBool4(this FixRandom random)                                           => new bool4(random.NextBool(), random.NextBool(), random.NextBool(), random.NextBool());
    public static FixVector2 NextFixVector2(this FixRandom random)                                 => new FixVector2(random.NextFix64(), random.NextFix64());
    public static FixVector2 NextFixVector2(this FixRandom random, FixVector2 max)                 => new FixVector2(random.NextFix64(max.x), random.NextFix64(max.y));
    public static FixVector2 NextFixVector2(this FixRandom random, FixVector2 min, FixVector2 max) => new FixVector2(random.NextFix64(min.x, max.x), random.NextFix64(min.y, max.y));
    public static FixVector3 NextFixVector3(this FixRandom random)                                 => new FixVector3(random.NextFix64(), random.NextFix64(), random.NextFix64());
    public static FixVector3 NextFixVector3(this FixRandom random, FixVector3 max)                 => new FixVector3(random.NextFix64(max.x), random.NextFix64(max.y), random.NextFix64(max.z));
    public static FixVector3 NextFixVector3(this FixRandom random, FixVector3 min, FixVector3 max) => new FixVector3(random.NextFix64(min.x, max.x), random.NextFix64(min.y, max.y), random.NextFix64(min.z, max.z));
    public static FixVector4 NextFixVector4(this FixRandom random)                                 => new FixVector4(random.NextFix64(), random.NextFix64(), random.NextFix64(), random.NextFix64());
    public static FixVector4 NextFixVector4(this FixRandom random, FixVector4 max)                 => new FixVector4(random.NextFix64(max.x), random.NextFix64(max.y), random.NextFix64(max.z), random.NextFix64(max.w));
    public static FixVector4 NextFixVector4(this FixRandom random, FixVector4 min, FixVector4 max) => new FixVector4(random.NextFix64(min.x, max.x), random.NextFix64(min.y, max.y), random.NextFix64(min.z, max.z), random.NextFix64(min.w, max.w));
    public static int2 NextInt2(this FixRandom random)                                             => new int2(random.NextInt(), random.NextInt());
    public static int2 NextInt2(this FixRandom random, int2 max)                                   => new int2(random.NextInt(max.x), random.NextInt(max.y));
    public static int2 NextInt2(this FixRandom random, int2 min, int2 max)                         => new int2(random.NextInt(min.x, max.x), random.NextInt(min.y, max.y));
    public static int3 NextInt3(this FixRandom random)                                             => new int3(random.NextInt(), random.NextInt(), random.NextInt());
    public static int3 NextInt3(this FixRandom random, int3 max)                                   => new int3(random.NextInt(max.x), random.NextInt(max.y), random.NextInt(max.z));
    public static int3 NextInt3(this FixRandom random, int3 min, int3 max)                         => new int3(random.NextInt(min.x, max.x), random.NextInt(min.y, max.y), random.NextInt(min.z, max.z));
    public static int4 NextInt4(this FixRandom random)                                             => new int4(random.NextInt(), random.NextInt(), random.NextInt(), random.NextInt());
    public static int4 NextInt4(this FixRandom random, int4 max)                                   => new int4(random.NextInt(max.x), random.NextInt(max.y), random.NextInt(max.z), random.NextInt(max.w));
    public static int4 NextInt4(this FixRandom random, int4 min, int4 max)                         => new int4(random.NextInt(min.x, max.x), random.NextInt(min.y, max.y), random.NextInt(min.z, max.z), random.NextInt(min.w, max.w));
    public static uint2 NextUInt2(this FixRandom random)                                           => new uint2(random.NextUInt(), random.NextUInt());
    public static uint3 NextUInt3(this FixRandom random)                                           => new uint3(random.NextUInt(), random.NextUInt(), random.NextUInt());
    public static uint4 NextUInt4(this FixRandom random)                                           => new uint4(random.NextUInt(), random.NextUInt(), random.NextUInt(), random.NextUInt());
    
    // methods that were not yet ported from Unity.Mathematic.Random
    //public static uint2 NextUInt2(this FixRandom random, uint2 max)                                => new uint2(random.NextUInt(max.x), random.NextUInt(max.y));
    //public static uint3 NextUInt3(this FixRandom random, uint3 max)                                => new uint3(random.NextUInt(max.x), random.NextUInt(max.y), random.NextUInt(max.z));
    //public static uint4 NextUInt4(this FixRandom random, uint4 max)                                => new uint4(random.NextUInt(max.x), random.NextUInt(max.y), random.NextUInt(max.z), random.NextUInt(max.w));
    //public static uint2 NextUInt2(this FixRandom random, uint2 min, uint2 max)                     => new uint2(random.NextUInt(), random.NextUInt());
    //public static uint3 NextUInt3(this FixRandom random, uint3 min, uint3 max)                     => new uint3(random.NextUInt(), random.NextUInt());
    //public static uint4 NextUInt4(this FixRandom random, uint4 min, uint4 max)                     => new uint4(random.NextUInt(), random.NextUInt());
    //public static quaternion NextQuaternionRotation(this FixRandom random);

    /// <summary>
    /// Vector will be normalized
    /// </summary>
    public static FixVector2 NextFixVector2Direction(this FixRandom random)
    {
        Fix64 angle = random.NextFix64Ratio() * Fix64.PiTimes2;

        return new FixVector2(
            Fix64.Cos(angle),   // x
            Fix64.Sin(angle));  // y
    }

    /// <summary>
    /// Vector will be normalized
    /// </summary>
    public static FixVector3 NextFixVector3Direction(this FixRandom random)
    {
        Fix64 phi = random.NextFix64Ratio() * Fix64.PiTimes2;
        Fix64 costheta = random.NextFix64(-1, 1);
        Fix64 theta = Fix64.Acos(costheta);

        return new FixVector3(
            Fix64.Sin(theta) * Fix64.Cos(phi), // x
            Fix64.Sin(theta) * Fix64.Sin(phi), // y
            Fix64.Cos(theta));                 // z
    }
}
