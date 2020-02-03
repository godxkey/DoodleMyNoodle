using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetMessageInterpreter
{
#if DEBUG_BUILD
    [ConfigVar(name: "log.netmessage_data", defaultValue: "0", ConfigVarFlag.Save, "Should we log the sent/received NetMessages.")]
    static ConfigVar s_logNetMessageData;
#endif

    public static bool GetMessageFromData(byte[] messageData, out object message)
    {
#if DEBUG_BUILD
        if (s_logNetMessageData.BoolValue)
        {
            DebugService.Log($"[NetMessage Interpretter] Receive Message byte[{messageData.Length}]");
            DebugLogUtility.LogByteArray(messageData);
        }
#endif
        BitStreamReader reader = new BitStreamReader(messageData);

        message = null;

        if (messageData.Length < 2) // 2 minimum bytes required for the message type
        {
            DebugService.LogError($"[NetMessageInterpreter] Error interpreting: data size to small ({messageData.Length} bytes)");
            return false;
        }

        try
        {
            message = DynamicNetSerializer.NetDeserialize(reader);

            return true;
        }
        catch (Exception e)
        {
            DebugService.LogError($"[NetMessageInterpreter] Failed to deserialize Message. {e.Message} - {e.StackTrace}");
            return false;
        }
    }

    public static bool GetDataFromMessage(object message, out byte[] data, int byteLimit = int.MaxValue)
    {
        int netBitSize = DynamicNetSerializer.GetNetBitSize(message);
        int messageSizeByte = netBitSize.CeiledToStep(8) / 8; // this will ceil the size to a multiple of 8

        if (messageSizeByte > byteLimit)
        {
            DebugService.LogError($"The net message is exceeding the {byteLimit} byte capacity.");
            data = null;
            return false;
        }
        data = new byte[messageSizeByte];

        BitStreamWriter writer = new BitStreamWriter(data);

        try
        {
            DynamicNetSerializer.NetSerialize(message, writer);

#if DEBUG_BUILD
            if (s_logNetMessageData.BoolValue)
            {
                DebugService.Log($"[NetMessageInterpreter] Serialize Message'{message}' to byte[{data.Length}]");
                DebugLogUtility.LogByteArray(data);
            }
#endif
            return true;
        }
        catch (Exception e)
        {
            DebugService.LogError($"[NetMessageInterpreter] Failed to serialize Message'{message}'. {e.Message} - {e.StackTrace}");
            return false;
        }
    }
}
