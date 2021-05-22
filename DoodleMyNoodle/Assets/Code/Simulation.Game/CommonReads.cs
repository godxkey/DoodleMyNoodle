using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using CCC.Fix2D;

public static partial class CommonReads
{
    public static bool IsInRange(ISimWorldReadAccessor accessor, Entity entityA, Entity entityB, fix rangeMax)
    {
        return fixMath.distance(
            accessor.GetComponent<FixTranslation>(entityB), 
            accessor.GetComponent<FixTranslation>(entityA)) < rangeMax;
    }
}