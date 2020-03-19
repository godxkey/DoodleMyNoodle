// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_SimulationControl_NetMessageSimTick
{
    public static int GetNetBitSize(ref SimulationControl.NetMessageSimTick obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimulationControl_SimTickData.GetNetBitSize(ref obj.TickData);
        return result;
    }

    public static void NetSerialize(ref SimulationControl.NetMessageSimTick obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimulationControl_SimTickData.NetSerialize(ref obj.TickData, writer);
    }

    public static void NetDeserialize(ref SimulationControl.NetMessageSimTick obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimulationControl_SimTickData.NetDeserialize(ref obj.TickData, reader);
    }
}
