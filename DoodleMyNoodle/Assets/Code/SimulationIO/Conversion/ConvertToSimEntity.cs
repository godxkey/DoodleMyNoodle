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


    private static fixQuaternion ToFix(in Quaternion fixQuat)
    {
        return new fixQuaternion((fix)fixQuat.x, (fix)fixQuat.y, (fix)fixQuat.z, (fix)fixQuat.w);
    }
    private static fix3 ToFix(in Vector3 vec)
    {
        return new fix3((fix)vec.x, (fix)vec.y, (fix)vec.z);
    }
}
