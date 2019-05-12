using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetMessageInterpreter
{
    public static object GetMessageFromData(byte[] messageData)
    {
        BitStreamReader reader = new BitStreamReader(messageData);

        if (messageData.Length < 4)
        {
            DebugService.LogError("[NetMessageInterpreter] Error interpreting: data size to small.");
            return null;
        }

        object message = DynamicNetSerializer.NetDeserialize(reader);

        if (message == null)
        {
            DebugService.LogError("[NetMessageInterpreter] Error interpreting: failed to create message.");
            return null;
        }

        return message;
    }

    public static void GetDataFromMessage(object message, out byte[] data)
    {
        int netBitSize = DynamicNetSerializer.GetNetBitSize(message);
        int messageSizeByte = netBitSize.CeiledToStep(8) / 8; // this will ceil the size to a multiple of 8

        data = new byte[messageSizeByte];

        if (data.Length > 512)
        {
            DebugService.LogError("The net message is exceeding the 512 byte capacity. Message fragmentation is a feature to be added.");
        }

        BitStreamWriter writer = new BitStreamWriter(data);

        DynamicNetSerializer.NetSerialize(message, writer);
    }
}
