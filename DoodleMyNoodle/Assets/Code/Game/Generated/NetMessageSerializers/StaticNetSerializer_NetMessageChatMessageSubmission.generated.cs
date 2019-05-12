// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageChatMessageSubmission
{
    public static int GetNetBitSize(ref NetMessageChatMessageSubmission obj)
    {
        int result = 0;
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.message);
        return result;
    }

    public static void NetSerialize(ref NetMessageChatMessageSubmission obj, BitStreamWriter writer)
    {
        StaticNetSerializer_String.NetSerialize(ref obj.message, writer);
    }

    public static void NetDeserialize(ref NetMessageChatMessageSubmission obj, BitStreamReader reader)
    {
        StaticNetSerializer_String.NetDeserialize(ref obj.message, reader);
    }
}
