using UnityEngine;

public static class FixVectorExtensions
{
    public static FixVector2 ToFixVec(this in Vector2 vec)
    {
        return new FixVector2((Fix64)vec.x, (Fix64)vec.y);
    }
    public static FixVector3 ToFixVec(this in Vector3 vec)
    {
        return new FixVector3((Fix64)vec.x, (Fix64)vec.y, (Fix64)vec.z);
    }
    public static FixVector4 ToFixVec(this in Vector4 vec)
    {
        return new FixVector4((Fix64)vec.x, (Fix64)vec.y, (Fix64)vec.z, (Fix64)vec.w);
    }
}