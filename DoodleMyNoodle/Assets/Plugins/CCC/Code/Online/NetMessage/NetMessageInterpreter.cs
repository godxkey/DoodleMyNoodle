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

        ushort messageType = reader.ReadUInt16();

        object message = NetMessageFactory.DeserializeMessage(messageType, reader);

        if (message == null)
        {
            DebugService.LogError("[NetMessageInterpreter] Error interpreting: failed to create message of type " + messageType);
            return null;
        }

        return message;
    }

    public static void GetDataFromMessage(object message, out byte[] data)
    {
        ushort messageType = NetMessageFactory.GetNetMessageTypeId(message);

        int netBitSize = NetMessageFactory.GetMessageBitSize(messageType, message);
        int messageSizeByte = netBitSize.CeiledToStep(8) / 8; // this will ceil the size to a multiple of 8

        data = new byte[messageSizeByte + 2 /* for the messageType*/ ];

        if (data.Length > 512)
        {
            DebugService.LogError("The net message is exceeding the 512 byte capacity. Message fragmentation is a feature to be added.");
        }

        BitStreamWriter writer = new BitStreamWriter(data);

        // message type
        writer.WriteUInt16(messageType);

        NetMessageFactory.SerializeMessage(messageType, message, writer);
    }
}
