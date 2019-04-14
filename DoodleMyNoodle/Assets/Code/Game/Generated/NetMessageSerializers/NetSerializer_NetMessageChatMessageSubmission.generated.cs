// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_NetMessageChatMessageSubmission
{
    public static int GetNetBitSize(ref NetMessageChatMessageSubmission obj)
    {
        int result = 0;
        result += NetSerializer_String.GetNetBitSize(ref obj.message);
        return result;
    }

    public static void NetSerialize(ref NetMessageChatMessageSubmission obj, BitStreamWriter writer)
    {
        NetSerializer_String.NetSerialize(ref obj.message, writer);
    }

    public static void NetDeserialize(ref NetMessageChatMessageSubmission obj, BitStreamReader reader)
    {
        NetSerializer_String.NetDeserialize(ref obj.message, reader);
    }
}
