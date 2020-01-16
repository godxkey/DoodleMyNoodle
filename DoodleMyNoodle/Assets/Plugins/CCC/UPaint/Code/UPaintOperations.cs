using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class UPaintOperations
{
    public static Color32 AlphaBlendColorOneOntoTwo(in Color32 one, in Color32 two)
    {
        return Color32.Lerp(two, one, (float)one.a / byte.MaxValue);
    }
}
