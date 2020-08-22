using static Unity.Mathematics.math;
using static fixMath;
using Unity.Mathematics;
using System.Diagnostics;

[DebuggerDisplay("{Min} : {Max}")]
public struct Aabb //  Axis-aligned bound box
{
    public fix2 Min;
    public fix2 Max;

    public fix2 Center => (fix)0.5f * (Max + Min);
    public fix2 Extents => Max - Min;
    public fix2 HalfExtents => (fix)0.5f * Extents;
    public bool IsValid => all(Min <= Max);

    public static Aabb Empty => new Aabb
    {
        Min = new fix2(fix.MaxValue),
        Max = new fix2(fix.MinValue),
    };

    public Aabb(fix2 point)
    {
        Min = point;
        Max = point;
    }

    public Aabb(fix2 min, fix2 max)
    {
        Min = min;
        Max = max;
    }

    public Aabb(Aabb aabb1, Aabb aabb2)
    {
        Min = min(aabb1.Min, aabb2.Min);
        Max = max(aabb1.Max, aabb2.Max);
    }

    public bool Contains(Aabb other)
    {
        bool result = true;
        result = result && Min.x <= other.Min.x;
        result = result && Min.y <= other.Min.y;
        result = result && other.Max.x <= Max.x;
        result = result && other.Max.y <= Max.y;
        return result;
    }

    public bool Overlap(Aabb aabb)
    {
        return all(Max >= aabb.Min & Min <= aabb.Max);
    }

    public bool Overlap(fix2 point)
    {
        return all(Max >= point & Min <= point);
    }

    public void Combine(Aabb other)
    {
        Min = min(Min, other.Min);
        Max = max(Max, other.Max);
    }

    public static Aabb Union(Aabb aabb1, Aabb aabb2)
    {
        aabb1.Include(aabb2);
        return aabb1;
    }

    public void Include(Aabb aabb)
    {
        Min = min(Min, aabb.Min);
        Max = max(Max, aabb.Max);
    }

    public void Include(fix2 point)
    {
        Min = min(Min, point);
        Max = max(Max, point);
    }

    public fix SurfaceArea
    {
        get
        {
            fix2 diff = Max - Min;
            return diff.x * diff.y;
        }
    }
}
