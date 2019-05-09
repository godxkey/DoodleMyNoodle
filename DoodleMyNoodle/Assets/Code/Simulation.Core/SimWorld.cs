using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimWorld
{
    public virtual void Tick_PreInput() { }
    public virtual void Tick_PostInput() { }
}
