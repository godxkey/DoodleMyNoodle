using UnityEngine;

public static class FixQuaternionExtensions
{
    public static Quaternion ToUnityQuat(this in fixQuaternion fixQuat)
    {
        return new Quaternion((float)fixQuat.x, (float)fixQuat.y, (float)fixQuat.z, (float)fixQuat.w);
    }
}