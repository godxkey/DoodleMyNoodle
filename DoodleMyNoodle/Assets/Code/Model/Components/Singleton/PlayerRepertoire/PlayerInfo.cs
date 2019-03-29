using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerInfo : INetSerializable
{
    public string playerName;
    public PlayerId playerId;

    public int valueInt;
    public uint valueUInt;
    public short valueShort;
    public ushort valueUShort;
    public bool valueBool;
    public byte valueByte;
}
