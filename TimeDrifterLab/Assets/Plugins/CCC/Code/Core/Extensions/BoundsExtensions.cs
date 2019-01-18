using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class BoundsExtensions
{
    public static void ClampPoint(this Bounds bounds, ref Vector3 point)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        point.x = Mathf.Clamp(point.x, min.x, max.x);
        point.y = Mathf.Clamp(point.y, min.y, max.y);
        point.z = Mathf.Clamp(point.z, min.z, max.z);
    }
}
