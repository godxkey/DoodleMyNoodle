using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
public struct MultiTimeValue
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
{
    public TimeValue Seconds;
    public TimeValue Turns;
    public TimeValue Rounds;

    public TimeValue GetValue(TimeValue.ValueType type)
    {
        switch (type)
        {
            case TimeValue.ValueType.Seconds:
                return Seconds;

            case TimeValue.ValueType.Turns:
                return Turns;

            case TimeValue.ValueType.Rounds:
                return Rounds;

            default:
                throw new System.Exception("Unknown time format");
        }
    }

    // maybe will be useful later ?
    //public static TimeValue operator +(MultiTimeValue x, TimeValue y) => x.GetValue(y.Type) + y;
    //public static TimeValue operator -(MultiTimeValue x, TimeValue y) => x.GetValue(y.Type) - y;
    //public static bool operator >(MultiTimeValue x, TimeValue y) => x.GetValue(y.Type) > y;
    //public static bool operator <(MultiTimeValue x, TimeValue y) => x.GetValue(y.Type) < y;
    //public static bool operator >=(MultiTimeValue x, TimeValue y) => x.GetValue(y.Type) >= y;
    //public static bool operator <=(MultiTimeValue x, TimeValue y) => x.GetValue(y.Type) <= y;
    //public static bool operator ==(MultiTimeValue x, TimeValue y) => x.GetValue(y.Type) == y;
    //public static bool operator !=(MultiTimeValue x, TimeValue y) => x.GetValue(y.Type) != y;
    //public static TimeValue operator +(TimeValue y, MultiTimeValue x) => x.GetValue(y.Type) + y;
    //public static TimeValue operator -(TimeValue y, MultiTimeValue x) => x.GetValue(y.Type) - y;
    //public static bool operator >(TimeValue y, MultiTimeValue x) => x.GetValue(y.Type) > y;
    //public static bool operator <(TimeValue y, MultiTimeValue x) => x.GetValue(y.Type) < y;
    //public static bool operator >=(TimeValue y, MultiTimeValue x) => x.GetValue(y.Type) >= y;
    //public static bool operator <=(TimeValue y, MultiTimeValue x) => x.GetValue(y.Type) <= y;
    //public static bool operator ==(TimeValue y, MultiTimeValue x) => x.GetValue(y.Type) == y;
    //public static bool operator !=(TimeValue y, MultiTimeValue x) => x.GetValue(y.Type) != y;
}

public abstract class SimGameSystemBase : SimSystemBase
{
    public new SimulationGameWorld World => (SimulationGameWorld)base.World;
    public PresentationEvents PresentationEvents => World.PresentationEvents;

    public MultiTimeValue GetElapsedTime()
    {
        return new MultiTimeValue()
        {
            Seconds = TimeValue.Seconds(World.FixTime.ElapsedTime),
            Turns = TimeValue.Turns(World.TurnTime.ElapsedTime),
            Rounds = TimeValue.Rounds(World.RoundTime.ElapsedTime),
        };
    }
    public MultiTimeValue GetDeltaTime()
    {
        return new MultiTimeValue()
        {
            Seconds = TimeValue.Seconds(World.FixTime.DeltaTime),
            Turns = TimeValue.Turns(World.TurnTime.DeltaTime),
            Rounds = TimeValue.Rounds(World.RoundTime.DeltaTime),
        };
    }

    public TimeValue GetElapsedTime(TimeValue.ValueType type)
    {
        switch (type)
        {
            case TimeValue.ValueType.Seconds:
                return TimeValue.Seconds(World.FixTime.ElapsedTime);

            case TimeValue.ValueType.Turns:
                return TimeValue.Turns(World.TurnTime.ElapsedTime);

            case TimeValue.ValueType.Rounds:
                return TimeValue.Rounds(World.RoundTime.ElapsedTime);

            default:
                throw new System.Exception("Unknown time format");
        }
    }

    public TimeValue GetDeltaTime(TimeValue.ValueType type)
    {
        switch (type)
        {
            case TimeValue.ValueType.Seconds:
                return TimeValue.Seconds(World.FixTime.DeltaTime);

            case TimeValue.ValueType.Turns:
                return TimeValue.Turns(World.TurnTime.DeltaTime);

            case TimeValue.ValueType.Rounds:
                return TimeValue.Rounds(World.RoundTime.DeltaTime);

            default:
                throw new System.Exception("Unknown time format");
        }
    }
}
