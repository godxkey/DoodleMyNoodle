// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageClientHello
{
    public static int GetNetBitSize(ref NetMessageClientHello obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.playerName);
        return result;
    }

    public static void NetSerialize(ref NetMessageClientHello obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.playerName, writer);
    }

    public static void NetDeserialize(ref NetMessageClientHello obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.playerName, reader);
    }
}
