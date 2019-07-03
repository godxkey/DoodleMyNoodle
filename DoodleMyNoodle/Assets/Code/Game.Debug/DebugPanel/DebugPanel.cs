using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebugPanel
{
    public bool isDisplayed { get; set; }

    public abstract string title { get; }
    public abstract bool canBeDisplayed { get; }

    public abstract void OnGUI();
    public virtual void OnStartDisplay() { }
    public virtual void OnStopDisplay() { }
}
