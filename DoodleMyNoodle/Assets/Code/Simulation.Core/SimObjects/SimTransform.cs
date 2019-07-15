using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SimTransform : SimComponent, ISimTickable
{
    public FixVector3 localScale = new FixVector3(1, 1, 1);
    public FixVector3 localPosition;
    public FixQuaternion localRotation;

    // TODO: worldPosition
    // TODO: worldRotation

    public SimTransform parent => unityTransform.parent?.GetComponent<SimTransform>();

    public void OnSimTick()
    {
        // could be optimized later
        unityTransform.localPosition = localPosition.ToUnityVec();
        unityTransform.localRotation = localRotation.ToUnityQuat();
        unityTransform.localScale = localScale.ToUnityVec();
    }
}
