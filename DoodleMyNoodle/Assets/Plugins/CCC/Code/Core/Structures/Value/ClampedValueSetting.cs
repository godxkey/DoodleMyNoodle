using System;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------------------------

// ClampedValueSetting - Used when a variable cannot go beneath or bellow certain values.

//-----------------------------------------------------------------------

[System.Serializable]
public struct ClampedValueSetting<T> : IValueEvaluator<T> where T : System.IComparable<T>
{
    public T MaxValue { get; private set; }
    public T MinValue { get; private set; }

    public T EvaluateValue(T currentValue)
    {
        T result = currentValue;
        if (currentValue.CompareTo(MaxValue) > 0)
            result = MaxValue;
        if (currentValue.CompareTo(MinValue) < 0)
            result = MinValue;
        return result;
    }
}