using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stat<T>
{
    private AutoResetDirtyValue<T> _dirtyValue;

    public Stat(T initialValue)
    {
        _dirtyValue = new AutoResetDirtyValue<T>(initialValue);
    }

    public T Value => _dirtyValue.Value;
    public T PreviousValue => _dirtyValue.PreviousValue;
    public bool HasChanged() => _dirtyValue.IsDirty;

    public bool SetValue(in T newValue)
    {
        return _dirtyValue.SetValue(newValue);
    }
}
