using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Unity.Mathematics.math; 
using static fixMath;
using Unity.Entities;
using Unity.Mathematics;

public static partial class Helpers
{
    public static fix2 ClampPositionInsideRange(fix2 position, fix2 centerPoint, fix range)
    {
        fix2 v = clampLength(position - centerPoint, 0, range);
        return centerPoint + v;
    }

    public static class AI
    {
        public static void FuzzifyThrow(ref fix throwAngle, ref fix throwSpeed, ref FixRandom random)
        {
            fix r = random.NextFixRatio();
            r *= r;

        }

        public static fix NextFuzzy(ref FixRandom random, fix min, fix max)
        {
            fix r = random.NextFixRatio();
            r *= r;

            r
        }
    }
}