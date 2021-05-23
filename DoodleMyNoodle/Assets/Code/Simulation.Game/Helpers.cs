using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using static fixMath;

public static partial class Helpers
{
    public static fix2 ClampPositionInsideRange(fix2 position, fix2 centerPoint, fix range)
    {
        fix2 v = clampLength(position - centerPoint, 0, range);
        return centerPoint + v;
    }
}