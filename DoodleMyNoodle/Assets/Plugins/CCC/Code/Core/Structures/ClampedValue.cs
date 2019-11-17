using System;
using System.Collections.Generic;

//-----------------------------------------------------------------------

// ClampedValue - Used when a variable cannot go beneath or bellow certain values. When changing the value it automaticly clamp

//-----------------------------------------------------------------------

public struct ClampedValue<T>
{
    // TODO : Support non Struct Values
}

//-----------------------------------------------------------------------

// Clamped Value Supporting a struct of INT
public struct ClampedStructValue<T> where T : IStructValue<int>
{
    public int maxValue;
    public int minValue;

    public T GetStruct { get; private set; }

    public void IncreaseValue(int newValue)
    {
        int newPossibleValue = GetStruct.GetValue() + newValue;

        if (newPossibleValue < minValue)
        {
            newPossibleValue = minValue;
        }
        else if (newPossibleValue > maxValue)
        {
            newPossibleValue = maxValue;
        }

        GetStruct.SetValue(newPossibleValue);
    }
}

//-----------------------------------------------------------------------

// Add more Clamped Struct Value Support HERE (ex: float, double, long, etc.)

//-----------------------------------------------------------------------

// TODO MOVE THIS IN ANOTHER SCRIPT
public interface IStructValue<T>
{
    T GetValue();
    void SetValue(T newValue);
}

//-----------------------------------------------------------------------