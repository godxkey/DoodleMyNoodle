using System;

public struct AutoResetDirtyValue<T>
{
    public T Value { get; private set; }
    public T PreviousValue { get; private set; }

    public AutoResetDirtyValue(T initialValue)
    {
        Value = initialValue;
        PreviousValue = Value;
    }

    public bool IsDirty
    {
        get
        {
            if (Value == null && PreviousValue == null)
                return false;
            if (Value == null || PreviousValue == null)
                return true;
            return !PreviousValue.Equals(Value);
        }
    }

    public void SetValue(in T newValue)
    {
        PreviousValue = Value;
        Value = newValue;
    }
}