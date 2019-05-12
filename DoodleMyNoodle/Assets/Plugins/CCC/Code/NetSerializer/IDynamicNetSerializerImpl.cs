using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDynamicNetSerializerImpl
{
    int     GetNetBitSize(object message);
    void    NetSerialize(object message, BitStreamWriter writer);
    object  NetDeserialize(BitStreamReader reader);
}
