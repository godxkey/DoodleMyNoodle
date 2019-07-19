using UnityEngine;

public static class FixQuaternionExtensions
{
    public static FixQuaternion ToFixQuat(this ref Quaternion fixQuat)
    {
        return new FixQuaternion((Fix64)fixQuat.x, (Fix64)fixQuat.y, (Fix64)fixQuat.z, (Fix64)fixQuat.w);
    }
    public static FixQuaternion ToFixQuatCopy(this Quaternion fixQuat)
    {
        return new FixQuaternion((Fix64)fixQuat.x, (Fix64)fixQuat.y, (Fix64)fixQuat.z, (Fix64)fixQuat.w);
    }
}