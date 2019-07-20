using UnityEngine;

public static class FixQuaternionExtensions
{
    public static Quaternion ToUnityQuat(this in FixQuaternion fixQuat)
    {
        return new Quaternion((float)fixQuat.X, (float)fixQuat.Y, (float)fixQuat.Z, (float)fixQuat.W);
    }
}