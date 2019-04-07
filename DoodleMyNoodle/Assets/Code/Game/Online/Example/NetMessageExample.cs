using System.Collections.Generic;

[NetMessage]
public class NetMessageExample
{
    public string valueString;
    public int valueInt;
    public uint valueUInt;
    public short valueShort;
    public ushort valueUShort;
    public bool valueBool;
    public byte valueByte;
    public int[] listOnInts;

    [NetMessageAttributes.Ignore]
    public bool ignoreThisField;
}