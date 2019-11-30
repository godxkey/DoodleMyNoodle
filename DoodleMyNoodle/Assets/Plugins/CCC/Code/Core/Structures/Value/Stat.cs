using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stat<T>
{
    public T startValue { get; set; }
    public T Value { get; private set; }

    public List<ValueSetting<T>> settings;

    public void SetValue(T newValue)
    {
        T valueToApply = newValue;
        foreach (ValueSetting<T> setting in settings)
        {
            valueToApply = setting.EvaluateValue(valueToApply);
        }
        Value = valueToApply;
    }
}
