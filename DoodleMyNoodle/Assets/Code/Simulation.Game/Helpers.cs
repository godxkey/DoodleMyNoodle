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
        //            ___________
        //          /      |      \
        //        /        |        \
        //      /          |          \
        //    /            |            \
        // -20°     -10°   0°  +10°     +20°
        private static readonly fix THROW_ANGLE_RANDOM_RANGE = rad(20);
        private static readonly fix THROW_ANGLE_RANDOM_PLATEAU_RATIO = fix.Half;

        //            ___________
        //          /      |      \
        //        /        |        \
        //      /          |          \
        //    /            |            \
        // -20%     -10%   0   +10%     +20%
        private static readonly fix THROW_SPEED_RANDOM_RANGE = (fix)0.2;
        private static readonly fix THROW_SPEED_RANDOM_PLATEAU_RATIO = fix.Half;

        public static void FuzzifyThrow(ref fix2 throwVector, ref FixRandom random)
        {
            fix angle = angle2d(throwVector);
            fix speed = length(throwVector);

            FuzzifyThrow(ref angle, ref speed, ref random);

            throwVector = new fix2(cos(angle) * speed, sin(angle) * speed);
        }

        public static void FuzzifyThrow(ref fix throwAngle, ref fix throwSpeed, ref FixRandom random)
        {
            throwAngle = random.NextPlateau(throwAngle - THROW_ANGLE_RANDOM_RANGE, throwAngle + THROW_ANGLE_RANDOM_RANGE, THROW_ANGLE_RANDOM_PLATEAU_RATIO);
            throwSpeed = random.NextPlateau(throwSpeed * (1 - THROW_SPEED_RANDOM_RANGE), throwSpeed * (1 + THROW_SPEED_RANDOM_RANGE), THROW_SPEED_RANDOM_PLATEAU_RATIO);
        }
    }
}