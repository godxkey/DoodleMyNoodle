using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimComponentViewTransform2D : SimComponentView<SimComponentTransform2D>
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
        simTransform = specificComponent;
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateSimFromSerializedData();
        }
    }

    // should be called by our custom inspector OnUpdate (todo)
    void UpdateSerializedDataFromSim()
    {
        SimComponentTransform2D comp = specificComponent;
        if (comp)
        {
            position = comp.position;
            rotation = comp.rotation;
        }
    }

    void UpdateSimFromSerializedData()
    {
        SimComponentTransform2D comp = specificComponent;
        if (comp)
        {
            comp.position = position;
            comp.rotation = rotation;
        }
    }
#endif

    protected override SimComponent CreateComponentFromSerializedData()
    {
        SimComponentTransform2D comp = new SimComponentTransform2D();

        comp.position = position;
        comp.rotation = rotation;

        return comp;
    }
}
