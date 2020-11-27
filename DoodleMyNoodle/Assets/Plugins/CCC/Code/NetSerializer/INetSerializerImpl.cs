using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetSerializerImpl
{
    ushort  GetTypeId(Type type);
    Type    GetTypeFromId(ushort typeId);
    Type    GetMessageType(BitStreamReader reader);
    bool    IsValidType(ushort typeId);
    bool    IsValidType(Type type);
    int     GetSerializedBitSize(object message);
    void    Serialize(object message, BitStreamWriter writer);
    object  Deserialize(BitStreamReader reader);
}
