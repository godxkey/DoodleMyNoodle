// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public partial struct PlayerId
{
    public int GetNetBitSize()
    {
        int result = 0;
        result += 16;
        return result;
    }

    public void NetSerialize(BitStreamWriter writer)
    {
        writer.WriteUInt16(value);
    }

    public void NetDeserialize(BitStreamReader reader)
    {
        value = reader.ReadUInt16();
    }
}
