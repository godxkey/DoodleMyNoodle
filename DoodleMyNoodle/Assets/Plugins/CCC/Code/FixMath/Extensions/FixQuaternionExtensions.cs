using UnityEngine;

public static class FixQuaternionExtensions
{
    public static Quaternion ToUnityQuat(this ref FixQuaternion fixQuat)
    {
        return new Quaternion((float)fixQuat.X, (float)fixQuat.Y, (float)fixQuat.Z, (float)fixQuat.W);
    }
    public static Quaternion ToUnityQuatCopy(this FixQuaternion fixQuat)
    {
        return new Quaternion((float)fixQuat.X, (float)fixQuat.Y, (float)fixQuat.Z, (float)fixQuat.W);
    }
}