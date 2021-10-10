using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public struct TimeValue
{
    public enum ValueType : Byte { Seconds, Turns, Rounds }

    public fix Value;
    public ValueType Type;
}