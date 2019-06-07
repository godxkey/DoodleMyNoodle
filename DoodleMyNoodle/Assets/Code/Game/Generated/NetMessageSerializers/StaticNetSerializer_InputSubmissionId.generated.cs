// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_InputSubmissionId
{
    public static int GetNetBitSize(ref InputSubmissionId obj)
    {
        int result = 0;
        result += StaticNetSerializer_Byte.GetNetBitSize(ref obj.value);
        return result;
    }

    public static void NetSerialize(ref InputSubmissionId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Byte.NetSerialize(ref obj.value, writer);
    }

    public static void NetDeserialize(ref InputSubmissionId obj, BitStreamReader reader)
    {
        StaticNetSerializer_Byte.NetDeserialize(ref obj.value, reader);
    }
}
