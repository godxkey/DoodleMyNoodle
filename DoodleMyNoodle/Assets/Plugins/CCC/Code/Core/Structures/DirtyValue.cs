using System;

public struct DirtyValue<T> : IStructValue<T>
{
    bool forceDirty;

    public T Value { get; private set; }
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

    public void SetValue(T newValue)
    {
        PreviousValue = Value;
        Value = newValue;
    }

    public T GetValue()
    {
        return Value;
    }

   void IStructValue<T>.SetValue(T newValue)
    {
        SetValue(newValue);
    }

    public static T operator +(DirtyValue<T> dirtyValue, T newValue)
    {
        dirtyValue.SetValue(newValue);
        return dirtyValue.Value;
    }
}