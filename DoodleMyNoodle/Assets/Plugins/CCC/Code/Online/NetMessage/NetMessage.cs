using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class NetMessage : INetSerializable
{
    public int TypeId { get; set; }

    public abstract int NetByteSize { get; }
    public abstract void NetDeserialize(BitStreamReader reader);
    public abstract void NetSerialize(BitStreamWriter writer);
}