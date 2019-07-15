using System;

[NetSerializable]
[Serializable]
public struct SimEntityId
{
    public static readonly SimEntityId invalid = new SimEntityId();
    public static readonly SimEntityId firstValid = new SimEntityId(1);

    public SimEntityId(SimEntityId other) { value = other.value; }
    public SimEntityId(UInt16 value) { this.value = value; }

    public UInt16 value;

    public void Increment()
    {
        value++;
    }
}
