using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebugPanel
{
    public bool IsDisplayed { get; set; }

    public abstract string Title { get; }
    public abstract bool CanBeDisplayed { get; }

    public abstract void OnGUI();
    public virtual void OnStartDisplay() { }
    public virtual void OnStopDisplay() { }
}
