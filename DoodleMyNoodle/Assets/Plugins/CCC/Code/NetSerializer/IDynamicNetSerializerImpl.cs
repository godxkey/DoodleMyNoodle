using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDynamicNetSerializerImpl
{
    Type    GetMessageType(BitStreamReader reader);
    bool    IsNetSerializable(Type type);
    int     GetNetBitSize(object message);
    void    NetSerialize(object message, BitStreamWriter writer);
    object  NetDeserialize(BitStreamReader reader);
}
