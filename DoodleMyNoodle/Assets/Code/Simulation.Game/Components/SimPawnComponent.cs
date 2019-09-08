using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entity that can be controlled
/// </summary>
public class SimPawnComponent : SimComponent
{
    public SimPlayerId PlayerInControl;

    public bool IsPossessed => PlayerInControl.IsValid;
}