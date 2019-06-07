using System;

[NetSerializable]
public struct SimPlayerId
{
    public static readonly SimPlayerId invalid = new SimPlayerId();
    public static readonly SimPlayerId firstValid = new SimPlayerId(1);

    public SimPlayerId(SimPlayerId other) { value = other.value; }
    public SimPlayerId(UInt16 value) { this.value = value; }

    public UInt16 value;

    public bool isValid => value != invalid.value;
}
