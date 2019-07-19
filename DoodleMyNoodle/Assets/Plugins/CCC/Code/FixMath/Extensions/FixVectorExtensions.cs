using UnityEngine;

public static class FixVectorExtensions
{
    public static Vector2 ToUnityVec(this ref FixVector2 fixVec)
    {
        return new Vector2((float)fixVec.X, (float)fixVec.Y);
    }
    public static Vector2 ToUnityVecCopy(this FixVector2 fixVec)
    {
        return new Vector2((float)fixVec.X, (float)fixVec.Y);
    }
    public static Vector3 ToUnityVec(this ref FixVector3 fixVec)
    {
        return new Vector3((float)fixVec.X, (float)fixVec.Y, (float)fixVec.Z);
    }
    public static Vector3 ToUnityVecCopy(this FixVector3 fixVec)
    {
        return new Vector3((float)fixVec.X, (float)fixVec.Y, (float)fixVec.Z);
    }
    public static Vector4 ToUnityVec(this ref FixVector4 fixVec)
    {
        return new Vector4((float)fixVec.X, (float)fixVec.Y, (float)fixVec.Z, (float)fixVec.W);
    }
    public static Vector4 ToUnityVecCopy(this FixVector4 fixVec)
    {
        return new Vector4((float)fixVec.X, (float)fixVec.Y, (float)fixVec.Z, (float)fixVec.W);
    }
}