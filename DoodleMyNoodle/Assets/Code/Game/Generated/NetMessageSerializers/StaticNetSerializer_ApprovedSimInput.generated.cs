// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_ApprovedSimInput
{
    public static int GetNetBitSize(ref ApprovedSimInput obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.playerInstigator);
        result += StaticNetSerializer_InputSubmissionId.GetNetBitSize(ref obj.clientSubmissionId);
        result += StaticNetSerializer_SimInput.GetNetBitSize_Class(obj.input);
        return result;
    }

    public static void NetSerialize(ref ApprovedSimInput obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.playerInstigator, writer);
        StaticNetSerializer_InputSubmissionId.NetSerialize(ref obj.clientSubmissionId, writer);
        StaticNetSerializer_SimInput.NetSerialize_Class(obj.input, writer);
    }

    public static void NetDeserialize(ref ApprovedSimInput obj, BitStreamReader reader)
    {
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.playerInstigator, reader);
        StaticNetSerializer_InputSubmissionId.NetDeserialize(ref obj.clientSubmissionId, reader);
        obj.input = StaticNetSerializer_SimInput.NetDeserialize_Class(reader);
    }
}
