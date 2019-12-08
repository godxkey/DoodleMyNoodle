
/// <summary>
/// A boolean that can only be affected in one way.
/// <para/>
/// E.g. Let's imagine a OneWayBool that starts as 'true'. If it set as 'false', it will not be able to go back to true.
/// </summary>
public struct OneWayBool
{
    public OneWayBool(bool startValue)
    {
        this.startValue = startValue;
        Value = startValue;
    }

    private bool startValue;

    /// <summary>
    /// Will set the value to the opposite of its starting value. Consequently, it has no effect if it's called multiple times
    /// </summary>
    public void FlipValue()
    {
        Value = !startValue;
    }
    public static implicit operator bool (OneWayBool val)
    {
        return val.Value;
    }
    public bool Value { get; private set; }

    /// <summary>
    /// Returns true if the value was successfuly set.
    /// </summary>
    public bool TryToSet(bool newValue)
    {
        if (Value == startValue)
        {
            Value = newValue;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool Equals(object obj)
    {
        if(obj is OneWayBool)
        {
            return ((OneWayBool)obj).Value == Value;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
