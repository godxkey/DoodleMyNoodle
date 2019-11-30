using System;

public struct DirtyValue<T>
{
    bool forceDirty;

    public T Value { get; set; }
    public T PreviousValue { get; private set; }

    public DirtyValue(T initialValue, bool dirtyOnStart = true)
    {
        Value = initialValue;
        PreviousValue = initialValue;
        forceDirty = dirtyOnStart;
    }

    public void Reset()
    {
        PreviousValue = Value;
        forceDirty = false;
    }

    public bool IsDirty
    {
        get
        {
            if (forceDirty)
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
        forceDirty = true;
    }
}