using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Mathematics;

public partial struct Helpers
{
    public static fix2 ClampPositionInsideRange(fix2 position, fix2 centerPoint, fix range)
    {
        fix2 v = clampLength(position - centerPoint, 0, range);
        return centerPoint + v;
    }
}