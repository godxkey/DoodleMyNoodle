using System;
using System.Collections.Generic;

/// <summary>
/// _value wrapper that easily identifies if a change in value has occured.
/// </summary>
public struct AutoResetDirtyValue<T>
{
    // fbessette:
    // This would be more intuitive as 'm_forcedDirty', but inverting it like that ensures the default value of the variable
    // matches with the design intention (being initially dirty by default)
    bool _notForcedDirty;

    T _previousValue;
    T _value;

    public T GetPrevious() => _previousValue;
    public T Get() => _value;

    /// <summary>
    /// Is the current value different from the previous one ?
    /// </summary>
    public bool IsDirty
    {
        get
        {
            if (!_notForcedDirty)
                return true;
            return !EqualityComparer<T>.Default.Equals(_previousValue, _value);
        }
    }

    public AutoResetDirtyValue(T initialValue, bool initiallyDirty = true)
    {
        _previousValue = initialValue;
        _value = initialValue;
        _notForcedDirty = !initiallyDirty;
    }

    public void Set(in T value)
    {
        Reset();
        _value = value;
    }

    /// <summary>
    /// Reset any dirtiness if there is any. Previous value will be equal to current value
    /// </summary>
    public void Reset()
    {
        _notForcedDirty = true;
        _previousValue = _value;
    }


    /// <summary>
    /// Forces the struct to be dirty, even if the current value and previous value are equal
    /// </summary>
    public void ForceDirty()
    {
        _notForcedDirty = false;
    }
}

/// <summary>
/// Reference wrapper that easily identifies if a change in reference has occured.
/// </summary>
public struct AutoResetDirtyRef<T> where T : class
{
    // fbessette:
    // This would be more intuitive as 'm_forcedDirty', but inverting it like that ensures the default value of the variable
    // matches with the design intention (being initially dirty by default)
    bool _notForcedDirty;

    T _previousRef;
    T _ref;

    public T GetPrevious() => _previousRef;
    public T Ref() => _ref;

    /// <summary>
    /// Is the current reference different from the previous one ?
    /// </summary>
    public bool IsDirty
    {
        get
        {
            if (!_notForcedDirty)
                return true;
            return ReferenceEquals(_previousRef, _ref);
        }
    }

    public AutoResetDirtyRef(T initialReference, bool initiallyDirty = true)
    {
        _previousRef = initialReference;
        _ref = initialReference;
        _notForcedDirty = !initiallyDirty;
    }

    public void Set(in T reference)
    {
        Reset();
        _ref = reference;
    }

    /// <summary>
    /// Reset any dirtiness if there is any. Previous ref will be equal to current ref
    /// </summary>
    public void Reset()
    {
        _notForcedDirty = true;
        _previousRef = _ref;
    }


    /// <summary>
    /// Forces the struct to be dirty, even if the current ref and previous ref are equal
    /// </summary>
    public void ForceDirty()
    {
        _notForcedDirty = false;
    }
}
