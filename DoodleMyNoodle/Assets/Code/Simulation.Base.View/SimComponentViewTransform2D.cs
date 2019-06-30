using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimComponentViewTransform2D : SimComponentView
{
    public float height;

    // cached references
    [System.NonSerialized]
    Transform tr;
    [System.NonSerialized]
    SimComponentTransform2D simTransform;

    private void Awake()
    {
        tr = transform;
    }

    public override void OnAttached()
    {
        base.OnAttached();
        simTransform = (SimComponentTransform2D)simComponent;
    }

    public override void OnDetached()
    {
        base.OnDetached();
        simTransform = null;
    }

    private void Update()
    {
        if (simTransform != null)
        {
            tr.position = new Vector3((float)simTransform.position.X, height, (float)simTransform.position.Y);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    //      Serialized Data                                 
    ////////////////////////////////////////////////////////////////////////////////////////

    [SerializeField] FixVector2 position;
    [SerializeField] Fix64 rotation;

    public override Type simComponentType => typeof(SimComponentTransform2D);

    protected override SimComponent CreateComponentFromSerializedData()
    {
        SimComponentTransform2D comp = new SimComponentTransform2D();

        ApplySerializedDataToSim(comp);

        return comp;
    }

#if UNITY_EDITOR
    protected override void ApplySimToSerializedData(SimComponent baseComp)
    {
        SimComponentTransform2D comp = (SimComponentTransform2D)baseComp;
        position = comp.position;
        rotation = comp.rotation;
    }
#endif
    protected override void ApplySerializedDataToSim(SimComponent baseComp)
    {
        SimComponentTransform2D comp = (SimComponentTransform2D)baseComp;
        comp.position = position;
        comp.rotation = rotation;
    }

}
