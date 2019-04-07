using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetMessageFactoryImpl
{
    ushort  GetNetMessageTypeId(object message);
    int     GetMessageBitSize(ushort messageType, object message);
    void    SerializeMessage(ushort messageType, object message, BitStreamWriter writer);
    object  DeserializeMessage(ushort messageType, BitStreamReader reader);
}
