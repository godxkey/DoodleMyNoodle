// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimInputSubmission
{
    public static int GetNetBitSize(ref SimInputSubmission obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt32.GetNetBitSize(ref obj.InstigatorConnectionId);
        result += StaticNetSerializer_InputSubmissionId.GetNetBitSize(ref obj.ClientSubmissionId);
        result += StaticNetSerializer_SimInput.GetNetBitSize_Class(obj.Input);
        return result;
    }

    public static void NetSerialize(ref SimInputSubmission obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt32.NetSerialize(ref obj.InstigatorConnectionId, writer);
        StaticNetSerializer_InputSubmissionId.NetSerialize(ref obj.ClientSubmissionId, writer);
        StaticNetSerializer_SimInput.NetSerialize_Class(obj.Input, writer);
    }

    public static void NetDeserialize(ref SimInputSubmission obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt32.NetDeserialize(ref obj.InstigatorConnectionId, reader);
        StaticNetSerializer_InputSubmissionId.NetDeserialize(ref obj.ClientSubmissionId, reader);
        obj.Input = StaticNetSerializer_SimInput.NetDeserialize_Class(reader);
    }
}
