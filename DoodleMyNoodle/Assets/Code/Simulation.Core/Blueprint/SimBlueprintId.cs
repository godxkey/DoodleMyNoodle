using System;

[NetSerializable]
public struct SimBlueprintId
{
    public static readonly SimBlueprintId invalid = new SimBlueprintId();
    public static readonly SimBlueprintId firstValid = new SimBlueprintId(1);

    public SimBlueprintId(SimBlueprintId other) { value = other.value; }
    public SimBlueprintId(UInt16 value) { this.value = value; }

    public UInt16 value;

    public bool isValid => value != invalid.value;
}
