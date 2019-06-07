using System;
using UnityEngine;


public class SimComponentView<T> : SimComponentView
    where T : SimComponent
{
    [SerializeField]
    T m_serializedComponent;
    internal override SimComponent serializedComponent => m_serializedComponent;


    public T specificComponent => (T)simComponent;
    public override Type simComponentType => typeof(T);


    public override string ToString()
    {
        return $"ComponentView<{typeof(T)}>({gameObject.name})";
    }

    public override void OnAttached()
    {
        base.OnAttached();
        m_serializedComponent = specificComponent;
    }

    public override void OnDetached()
    {
        base.OnDetached();
        m_serializedComponent = null;
    }
}

public abstract class SimComponentView : SimObjectView
{
    public SimComponent simComponent => (SimComponent)simObject;
    public abstract Type simComponentType { get; }
    internal abstract SimComponent serializedComponent { get; }
}