using System;

[Serializable]
public class SimObject
{
    [NonSerialized]
    SimObjectView m_view;
    public SimObjectView view { get => m_view; internal set => m_view = value; }
    public bool attachedToView => view != null;



    internal SimObject()
    {

    }

    public virtual void OnAwake() { }
    //public virtual void OnStart() { } not implemented yet
    public virtual void OnDestroy() { }
}
