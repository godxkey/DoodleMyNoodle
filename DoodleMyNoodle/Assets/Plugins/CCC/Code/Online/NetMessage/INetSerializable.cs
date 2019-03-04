using System;

public interface INetSerializable
{
    int NetByteSize { get; }
    void NetSerialize(BitStreamWriter writer);
    void NetDeserialize(BitStreamReader reader);
}