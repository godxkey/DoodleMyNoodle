using UnityEngine;

public static class FixVectorExtensions
{
    public static fix2 ToFixVec(this in Vector2 vec)
    {
        return new fix2((fix)vec.x, (fix)vec.y);
    }
    public static fix3 ToFixVec(this in Vector3 vec)
    {
        return new fix3((fix)vec.x, (fix)vec.y, (fix)vec.z);
    }
    public static fix4 ToFixVec(this in Vector4 vec)
    {
        return new fix4((fix)vec.x, (fix)vec.y, (fix)vec.z, (fix)vec.w);
    }
}