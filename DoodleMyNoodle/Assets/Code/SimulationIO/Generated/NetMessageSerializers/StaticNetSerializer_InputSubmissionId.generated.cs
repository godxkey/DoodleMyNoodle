// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_InputSubmissionId
{
    public static int GetNetBitSize(ref InputSubmissionId obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Byte.GetNetBitSize(ref obj.value);
        return result;
    }

    public static void NetSerialize(ref InputSubmissionId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Byte.NetSerialize(ref obj.value, writer);
    }

    public static void NetDeserialize(ref InputSubmissionId obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Byte.NetDeserialize(ref obj.value, reader);
    }
}
