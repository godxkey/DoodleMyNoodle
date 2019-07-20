using UnityEngine;

public static class FixQuaternionExtensions
{
    public static FixQuaternion ToFixQuat(this in Quaternion fixQuat)
    {
        return new FixQuaternion((Fix64)fixQuat.x, (Fix64)fixQuat.y, (Fix64)fixQuat.z, (Fix64)fixQuat.w);
    }
}