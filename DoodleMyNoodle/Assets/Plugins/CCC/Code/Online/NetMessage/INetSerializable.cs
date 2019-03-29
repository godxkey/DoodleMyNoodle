using System;

public interface INetSerializable
{
    int GetNetBitSize();
    void NetSerialize(BitStreamWriter writer);
    void NetDeserialize(BitStreamReader reader);
}