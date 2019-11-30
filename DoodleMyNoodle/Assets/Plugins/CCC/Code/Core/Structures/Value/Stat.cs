using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stat<T>
{
    private T _startValue;
    private AutoResetDirtyValue<T> _dirtyValue;

    public Stat(T initialValue)
    {
        _startValue = initialValue;
        _dirtyValue = new AutoResetDirtyValue<T>(initialValue);
    }

    public void Reset()
    {
        _dirtyValue.SetValue(_startValue);
    }

    public bool SetValue(T newValue)
    {
        return _dirtyValue.SetValue(newValue);
    }

    public AutoResetDirtyValue<T> GetValue()
    {
        return _dirtyValue;
    }
}
