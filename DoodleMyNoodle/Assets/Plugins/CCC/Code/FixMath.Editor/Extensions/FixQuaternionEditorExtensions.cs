using UnityEngine;

public static class FixQuaternionExtensions
{
    public static fixQuaternion ToFixQuat(this in Quaternion fixQuat)
    {
        return new fixQuaternion((fix)fixQuat.x, (fix)fixQuat.y, (fix)fixQuat.z, (fix)fixQuat.w);
    }
}