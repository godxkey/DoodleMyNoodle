using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stat<T>
{
    private AutoResetDirtyValue<T> _dirtyValue;

    public Stat(T initialValue)
    {
        _dirtyValue = new AutoResetDirtyValue<T>(initialValue);
    }

    public T Value => _dirtyValue.Get();
    public T PreviousValue => _dirtyValue.GetPrevious();
    public bool HasChanged() => _dirtyValue.IsDirty;

    public void SetValue(in T newValue)
    {
        _dirtyValue.Set(newValue);
    }
}
