using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ConvertToSimEntity : ConvertToEntityMultiWorld
{
    public override GameWorldType WorldToConvertTo => GameWorldType.Simulation;

    protected override void Awake()
    {
        FillLocalFixTransformData();

        base.Awake();
    }

    public void FillLocalFixTransformData()
    {
        if (gameObject.HasComponent<NoTransform>())
            return;

        FixTransformAuth fixTransform = gameObject.GetOrAddComponent<FixTransformAuth>();
        Transform tr = transform;

        // we must use "local space data" instead of "world space data" because using world space would
        //  mean using unity's matrix calculations, which is non-deterministic (using floats)
        fixTransform.LocalPosition = ToFix(tr.localPosition);
        fixTransform.LocalRotation = ToFix(tr.localRotation);
        fixTransform.LocalScale = ToFix(tr.localScale);
    }


    private static FixQuaternion ToFix(in Quaternion fixQuat)
    {
        return new FixQuaternion((Fix64)fixQuat.x, (Fix64)fixQuat.y, (Fix64)fixQuat.z, (Fix64)fixQuat.w);
    }
    private static FixVector3 ToFix(in Vector3 vec)
    {
        return new FixVector3((Fix64)vec.x, (Fix64)vec.y, (Fix64)vec.z);
    }
}
