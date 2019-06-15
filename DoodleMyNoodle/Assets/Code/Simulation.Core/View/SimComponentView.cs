using System;
using UnityEngine;


public abstract class SimComponentView<T> : SimComponentView
    where T : SimComponent
{
    public T specificComponent => (T)simComponent;
    public override Type simComponentType => typeof(T);


    public override string ToString()
    {
        return $"ComponentView<{typeof(T)}>({gameObject.name})";
    }

    public override void OnAttached()
    {
        base.OnAttached();
    }

    public override void OnDetached()
    {
        base.OnDetached();
    }
}

public abstract class SimComponentView : SimObjectView
{
    public SimComponent simComponent => (SimComponent)simObject;
    public abstract Type simComponentType { get; }

    protected abstract SimComponent CreateComponentFromSerializedData();

    internal SimComponent GetComponentFromSerializedData() => CreateComponentFromSerializedData();
}