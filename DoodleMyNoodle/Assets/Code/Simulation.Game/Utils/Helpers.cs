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
        public static void FuzzifyThrow(ref fix2 throwVector, ref FixRandom random, in AIFuzzyThrowSettings fuzzySettings)
        {
            fix angle = angle2d(throwVector);
            fix speed = length(throwVector);

            FuzzifyThrow(ref angle, ref speed, ref random, fuzzySettings);

            throwVector = new fix2(cos(angle) * speed, sin(angle) * speed);
        }

        public static void FuzzifyThrow(ref fix throwAngle, ref fix throwSpeed, ref FixRandom random, in AIFuzzyThrowSettings fuzzySettings)
        {
            throwSpeed *= fuzzySettings.ThrowSpeed.GetNext(ref random);
            throwAngle += fuzzySettings.ThrowAngle.GetNext(ref random);
        }
    }
}