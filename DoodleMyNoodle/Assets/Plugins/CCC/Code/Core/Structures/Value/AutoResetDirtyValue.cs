using System;

public struct AutoResetDirtyValue<T>
{
    public T Value { get; private set; }
    private T PreviousValue { get; set; }

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

    public bool SetValue(T newValue)
    {
        PreviousValue = Value;
        Value = newValue;
        return IsDirty;
    }
}