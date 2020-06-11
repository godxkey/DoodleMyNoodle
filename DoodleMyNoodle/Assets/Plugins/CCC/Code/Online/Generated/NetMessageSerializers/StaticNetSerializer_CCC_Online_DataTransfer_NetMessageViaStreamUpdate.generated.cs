// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamUpdate
{
    public static int GetNetBitSize(ref CCC.Online.DataTransfer.NetMessageViaStreamUpdate obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Single.GetNetBitSize(ref obj.Progress);
        return result;
    }

    public static void NetSerialize(ref CCC.Online.DataTransfer.NetMessageViaStreamUpdate obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Single.NetSerialize(ref obj.Progress, writer);
    }

    public static void NetDeserialize(ref CCC.Online.DataTransfer.NetMessageViaStreamUpdate obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Single.NetDeserialize(ref obj.Progress, reader);
    }
}
