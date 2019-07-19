using UnityEngine;

public static class FixVectorExtensions
{
    public static FixVector2 ToFixVec(this ref Vector2 vec)
    {
        return new FixVector2((Fix64)vec.x, (Fix64)vec.y);
    }
    public static FixVector2 ToFixVecCopy(this Vector2 vec)
    {
        return new FixVector2((Fix64)vec.x, (Fix64)vec.y);
    }
    public static FixVector3 ToFixVec(this ref Vector3 vec)
    {
        return new FixVector3((Fix64)vec.x, (Fix64)vec.y, (Fix64)vec.z);
    }
    public static FixVector3 ToFixVecCopy(this Vector3 vec)
    {
        return new FixVector3((Fix64)vec.x, (Fix64)vec.y, (Fix64)vec.z);
    }
    public static FixVector4 ToFixVec(this ref Vector4 vec)
    {
        return new FixVector4((Fix64)vec.x, (Fix64)vec.y, (Fix64)vec.z, (Fix64)vec.w);
    }
    public static FixVector4 ToFixVecCopy(this Vector4 vec)
    {
        return new FixVector4((Fix64)vec.x, (Fix64)vec.y, (Fix64)vec.z, (Fix64)vec.w);
    }
}