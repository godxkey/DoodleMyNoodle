using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebugPanel
{
    public abstract string Title { get; }
    public abstract bool CanBeDisplayed { get; }
    public abstract void OnGUI();
}
