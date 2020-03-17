// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageInputSubmission
{
    public static int GetNetBitSize(ref NetMessageInputSubmission obj)
    {
        int result = 0;
        result += StaticNetSerializer_InputSubmissionId.GetNetBitSize(ref obj.submissionId);
        result += StaticNetSerializer_SimInput.GetNetBitSize_Class(obj.input);
        return result;
    }

    public static void NetSerialize(ref NetMessageInputSubmission obj, BitStreamWriter writer)
    {
        StaticNetSerializer_InputSubmissionId.NetSerialize(ref obj.submissionId, writer);
        StaticNetSerializer_SimInput.NetSerialize_Class(obj.input, writer);
    }

    public static void NetDeserialize(ref NetMessageInputSubmission obj, BitStreamReader reader)
    {
        StaticNetSerializer_InputSubmissionId.NetDeserialize(ref obj.submissionId, reader);
        obj.input = StaticNetSerializer_SimInput.NetDeserialize_Class(reader);
    }
}
