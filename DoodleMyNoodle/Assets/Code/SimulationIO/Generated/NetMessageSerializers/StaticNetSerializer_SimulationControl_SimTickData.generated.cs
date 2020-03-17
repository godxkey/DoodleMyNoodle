// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimulationControl_SimTickData
{
    public static int GetNetBitSize(ref SimulationControl.SimTickData obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt32.GetNetBitSize(ref obj.ExpectedNewTickId);
        result += ArrayNetSerializer_SimInputSubmission.GetNetBitSize(ref obj.InputSubmissions);
        return result;
    }

    public static void NetSerialize(ref SimulationControl.SimTickData obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt32.NetSerialize(ref obj.ExpectedNewTickId, writer);
        ArrayNetSerializer_SimInputSubmission.NetSerialize(ref obj.InputSubmissions, writer);
    }

    public static void NetDeserialize(ref SimulationControl.SimTickData obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt32.NetDeserialize(ref obj.ExpectedNewTickId, reader);
        ArrayNetSerializer_SimInputSubmission.NetDeserialize(ref obj.InputSubmissions, reader);
    }
}
