using UnityEngine;

public class SimObjectView : MonoBehaviour
{
    public SimObject simObject { get; internal set; }
    public bool attachedToSim => simObject != null;

    public virtual void OnAttached() { }
    public virtual void OnDetached() { }
}