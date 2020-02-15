using System;

[System.Serializable]
public struct AutoResetDirtyValue<T>
{
    private T _value;
    private T _previousValue;

    public T Value => _value;
    public T PreviousValue => _previousValue;

    public AutoResetDirtyValue(T initialValue)
    {
        _value = initialValue;
        _previousValue = _value;
    }

    public bool IsDirty
    {
        get
        {
            if (_value == null && _previousValue == null)
                return false;
            if (_value == null || _previousValue == null)
                return true;
            return !_previousValue.Equals(_value);
        }
    }

    public void SetValue(in T newValue)
    {
        _previousValue = _value;
        _value = newValue;
    }
}