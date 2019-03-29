using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetMessage : INetSerializable
{
    public virtual int GetNetBitSize() => 0;
    public virtual void NetDeserialize(BitStreamReader reader) { }
    public virtual void NetSerialize(BitStreamWriter writer) { }
}