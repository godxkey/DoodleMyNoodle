using System;

public struct DirtyValue<T>
{
    bool _forceDirty;

    public T Value { get; set; }
    public T PreviousValue { get; private set; }

    public DirtyValue(T initialValue, bool dirtyOnStart = true)
    {
        Value = initialValue;
        PreviousValue = initialValue;
        _forceDirty = dirtyOnStart;
    }

    public void Reset()
    {
        PreviousValue = Value;
        _forceDirty = false;
    }

    public bool IsDirty
    {
        get
        {
            if (_forceDirty)
                return true;
            if (Value == null && PreviousValue == null)
                return false;
            if (Value == null || PreviousValue == null)
                return true;
            return !PreviousValue.Equals(Value);
        }
    }

    public void ForceDirty()
    {
        _forceDirty = true;
    }
}