using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngineX;
using System.Diagnostics;

[System.Serializable]
public struct TimeValue : IEquatable<TimeValue>, IComparable<TimeValue>
{
    public enum ValueType : Byte { Seconds, Turns, Rounds }

    public ValueType Type;
    public fix Value;

    public static readonly TimeValue Zero = default;

    public static TimeValue Seconds(fix value) => new TimeValue() { Value = value, Type = ValueType.Seconds };
    public static TimeValue Turns(fix value) => new TimeValue() { Value = value, Type = ValueType.Turns };
    public static TimeValue Rounds(fix value) => new TimeValue() { Value = value, Type = ValueType.Rounds };

    public static TimeValue operator +(TimeValue x, TimeValue y)
    {
        ErrorIfTypeMismatch(x, y);

        return new TimeValue()
        {
            Value = x.Value + y.Value,
            Type = x.Type
        };
    }

    public static TimeValue operator -(TimeValue x, TimeValue y)
    {
        ErrorIfTypeMismatch(x, y);

        return new TimeValue()
        {
            Value = x.Value - y.Value,
            Type = x.Type
        };
    }

    public static bool operator >(TimeValue x, TimeValue y)
    {
        ErrorIfTypeMismatch(x, y);

        return x.Value > y.Value;
    }

    public static bool operator <(TimeValue x, TimeValue y)
    {
        ErrorIfTypeMismatch(x, y);

        return x.Value < y.Value;
    }

    public static bool operator >=(TimeValue x, TimeValue y)
    {
        ErrorIfTypeMismatch(x, y);

        return x.Value >= y.Value;
    }

    public static bool operator <=(TimeValue x, TimeValue y)
    {
        ErrorIfTypeMismatch(x, y);

        return x.Value <= y.Value;
    }

    public static bool operator ==(TimeValue x, TimeValue y)
    {
        ErrorIfTypeMismatch(x, y);

        return x.Value == y.Value;
    }

    public static bool operator !=(TimeValue x, TimeValue y)
    {
        ErrorIfTypeMismatch(x, y);

        return x.Value != y.Value;
    }

    public int CompareTo(TimeValue other)
    {
        ErrorIfTypeMismatch(this, other);

        return Value.CompareTo(other.Value);
    }

    [Conditional("SAFETY")]
    private static void ErrorIfTypeMismatch(TimeValue x, TimeValue y)
    {
        if (x.Type != y.Type && x.Value != 0 && y.Value != 0)
            throw new Exception();
    }

    public override bool Equals(object obj)
    {
        return obj is TimeValue value && Equals(value);
    }

    public bool Equals(TimeValue other)
    {
        return Value.Equals(other.Value) &&
               Type == other.Type;
    }

    public override int GetHashCode()
    {
        int hashCode = 1574892647;
        hashCode = hashCode * -1521134295 + Value.GetHashCode();
        hashCode = hashCode * -1521134295 + Type.GetHashCode();
        return hashCode;
    }
}