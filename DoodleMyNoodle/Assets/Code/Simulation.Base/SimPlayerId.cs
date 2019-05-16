using System;

[NetSerializable]
public struct SimPlayerId
{
    public static readonly SimPlayerId invalid = new SimPlayerId(UInt16.MaxValue);

    public SimPlayerId(SimPlayerId other) { value = other.value; }
    public SimPlayerId(UInt16 value) { this.value = value; }

    public UInt16 value;

    public bool isValid => value != invalid.value;
}
