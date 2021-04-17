using Unity.Entities;
using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Jobs;

public struct Signal : IComponentData
{
    public bool Value;

    public static implicit operator bool(Signal val) => val.Value;
    public static implicit operator Signal(bool val) => new Signal() { Value = val };
}

public struct PreviousSignal : IComponentData
{
    public bool Value;

    public static implicit operator bool(PreviousSignal val) => val.Value;
    public static implicit operator PreviousSignal(bool val) => new PreviousSignal() { Value = val };
}

public enum ESignalEmissionType
{
    /// <summary>
    /// Signal never turns on by itself
    /// </summary>
    None,

    /// <summary>
    /// Signal stays ON while entity overlaps.
    /// </summary>
    WhileOverlap,

    /// <summary>
    /// Signal ON for 1 frame after OnEnter.
    /// </summary>
    OnEnter,

    /// <summary>
    /// Signal ON for 1 frame after click.
    /// </summary>
    OnClick,

    /// <summary>
    /// Signal stays ON/OFF, changes at every click. 
    /// </summary>
    ToggleOnClick
}

//public struct SignalEmission : IComponentData
//{
//    public bool Value;

//    public static implicit operator bool(SignalEmission val) => val.Value;
//    public static implicit operator SignalEmission(bool val) => new SignalEmission() { Value = val };
//}

public struct SignalEmissionType : IComponentData
{
    public ESignalEmissionType Value;

    public static implicit operator ESignalEmissionType(SignalEmissionType val) => val.Value;
    public static implicit operator SignalEmissionType(ESignalEmissionType val) => new SignalEmissionType() { Value = val };
}

[InternalBufferCapacity(1)] // very un-usual for more than 1 target
public struct SignalPropagationTarget : IBufferElementData
{
    public Entity Value;

    public static implicit operator Entity(SignalPropagationTarget val) => val.Value;
    public static implicit operator SignalPropagationTarget(Entity val) => new SignalPropagationTarget() { Value = val };
}

public enum ESignalEmissionFlags
{
    Overlapping = 1 << 0,
    ToggleClickOn = 1 << 1,
}

public struct SignalEmissionFlags : IComponentData
{
    public ESignalEmissionFlags Value;

    public static implicit operator ESignalEmissionFlags(SignalEmissionFlags val) => val.Value;
    public static implicit operator SignalEmissionFlags(ESignalEmissionFlags val) => new SignalEmissionFlags() { Value = val };

    public bool Overlapping
    {
        get => (Value & ESignalEmissionFlags.Overlapping) != 0;
        set => Value = value ? Value | ESignalEmissionFlags.Overlapping : Value & ~ESignalEmissionFlags.Overlapping;
    }
    public bool ToggleClickOn
    {
        get => (Value & ESignalEmissionFlags.ToggleClickOn) != 0;
        set => Value = value ? Value | ESignalEmissionFlags.ToggleClickOn : Value & ~ESignalEmissionFlags.ToggleClickOn;
    }
}