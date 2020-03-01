using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDynamicNetSerializerImpl
{
    ushort  GetTypeId(Type type);
    Type    GetTypeFromId(ushort typeId);
    Type    GetMessageType(BitStreamReader reader);
    bool    IsValidType(ushort typeId);
    bool    IsValidType(Type type);
    int     GetNetBitSize(object message);
    void    NetSerialize(object message, BitStreamWriter writer);
    object  NetDeserialize(BitStreamReader reader);
}
