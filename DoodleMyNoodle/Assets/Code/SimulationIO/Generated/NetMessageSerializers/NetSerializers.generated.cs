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
public static class StaticNetSerializer_NetMessageAcceptSimSync
{
    public static int GetNetBitSize(ref NetMessageAcceptSimSync obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.TransferByDisk);
        return result;
    }

    public static void NetSerialize(ref NetMessageAcceptSimSync obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.TransferByDisk, writer);
    }

    public static void NetDeserialize(ref NetMessageAcceptSimSync obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.TransferByDisk, reader);
    }
}
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
public static class StaticNetSerializer_NetMessageRequestSimSync
{
    public static int GetNetBitSize(ref NetMessageRequestSimSync obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.AttemptTransferByDisk);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.LocalMachineName);
        return result;
    }

    public static void NetSerialize(ref NetMessageRequestSimSync obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.AttemptTransferByDisk, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.LocalMachineName, writer);
    }

    public static void NetDeserialize(ref NetMessageRequestSimSync obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.AttemptTransferByDisk, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.LocalMachineName, reader);
    }
}
public static class StaticNetSerializer_NetMessageSerializedSimulation
{
    public static int GetNetBitSize(ref NetMessageSerializedSimulation obj)
    {
        int result = 0;
        result += ArrayNetSerializer_System_Byte.GetNetBitSize(ref obj.SerializedSimulation);
        return result;
    }

    public static void NetSerialize(ref NetMessageSerializedSimulation obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_System_Byte.NetSerialize(ref obj.SerializedSimulation, writer);
    }

    public static void NetDeserialize(ref NetMessageSerializedSimulation obj, BitStreamReader reader)
    {
        ArrayNetSerializer_System_Byte.NetDeserialize(ref obj.SerializedSimulation, reader);
    }
}
public static class StaticNetSerializer_NetMessageSimSyncFromFile
{
    public static int GetNetBitSize(ref NetMessageSimSyncFromFile obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.SerializedSimulationFilePath);
        return result;
    }

    public static void NetSerialize(ref NetMessageSimSyncFromFile obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.SerializedSimulationFilePath, writer);
    }

    public static void NetDeserialize(ref NetMessageSimSyncFromFile obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.SerializedSimulationFilePath, reader);
    }
}
public static class StaticNetSerializer_SimCommandLoadScene
{
    public static int GetNetBitSize_Class(SimCommandLoadScene obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimCommandLoadScene obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.SceneName);
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimCommandLoadScene obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimCommandLoadScene obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.SceneName, writer);
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimCommandLoadScene NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimCommandLoadScene obj = new SimCommandLoadScene();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimCommandLoadScene obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.SceneName, reader);
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
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
public static class StaticNetSerializer_SimulationControl_SimTickData
{
    public static int GetNetBitSize(ref SimulationControl.SimTickData obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt32.GetNetBitSize(ref obj.ExpectedNewTickId);
        result += ListNetSerializer_SimInputSubmission.GetNetBitSize_Class(obj.InputSubmissions);
        return result;
    }

    public static void NetSerialize(ref SimulationControl.SimTickData obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt32.NetSerialize(ref obj.ExpectedNewTickId, writer);
        ListNetSerializer_SimInputSubmission.NetSerialize_Class(obj.InputSubmissions, writer);
    }

    public static void NetDeserialize(ref SimulationControl.SimTickData obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt32.NetDeserialize(ref obj.ExpectedNewTickId, reader);
        obj.InputSubmissions = ListNetSerializer_SimInputSubmission.NetDeserialize_Class(reader);
    }
}

public static class ListNetSerializer_SimInputSubmission
{
    public static int GetNetBitSize_Class(List<SimInputSubmission> obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Count; i++)
        {
            var x = obj[i];
            result += StaticNetSerializer_SimInputSubmission.GetNetBitSize(ref x);
        }
        return result;
    }

    public static void NetSerialize_Class(List<SimInputSubmission> obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Count);
        for (int i = 0; i < obj.Count; i++)
        {
            var x = obj[i];
            StaticNetSerializer_SimInputSubmission.NetSerialize(ref x, writer);
        }
    }

    public static List<SimInputSubmission> NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        int size = reader.ReadInt32();
        List<SimInputSubmission> obj = new List<SimInputSubmission>(size);
        for (int i = 0; i < size; i++)
        {
            SimInputSubmission x = default;
            StaticNetSerializer_SimInputSubmission.NetDeserialize(ref x, reader);
            obj.Add(x);
        }
        return obj;
    }
}
