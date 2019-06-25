using System;
using UnityEngine;

[DisallowMultipleComponent]
public class SimEntityView : SimObjectView
{
    public bool destroyOnDetach;

    [ShowIf(nameof(destroyOnDetach))]
    public float destroyDelay = 0;

    public SimEntity simEntity => (SimEntity)simObject;

    public SimComponentView[] GetComponentViews()
    {
        return GetComponents<SimComponentView>();
    }

    public override string ToString()
    {
        return $"EntityView({gameObject.name})";
    }
}