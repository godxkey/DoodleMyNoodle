using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetMessageExample : NetMessage
{
    public string valueString;
    public int valueInt;
    public uint valueUInt;
    public short valueShort;
    public ushort valueUShort;
    public bool valueBool;
    public byte valueByte;
    public List<int> listOfNetSerializableValue = new List<int>(); // need to instantiate list

    [NetMessageAttributes.Ignore]
    public bool ignoreThisField;
}

[NetMessageAttributes.Ignore]
public class IgnoreThisClass : NetMessage
{

}
