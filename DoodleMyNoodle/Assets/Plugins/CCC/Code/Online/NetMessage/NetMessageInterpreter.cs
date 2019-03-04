using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetMessageInterpreter
{
    public static NetMessage GetMessageFromData(byte[] messageData)
    {
        BitStreamReader reader = new BitStreamReader(messageData);

        if(messageData.Length < 4)
        {
            Debug.LogError("[NetMessageInterpreter] Error interpreting: data size to small.");
            return null;
        }

        ushort messageType = reader.ReadUInt16();

        NetMessage message = NetMessageFactory.CreateNetMessage(messageType);

        if(message == null)
        {
            Debug.LogError("[NetMessageInterpreter] Error interpreting: failed to create message of type " + messageType);
            return null;
        }

        message.NetDeserialize(reader);

        return message;
    }

    public static void GetDataFromMessage(NetMessage message, out byte[] data)
    {
        data = new byte[message.NetByteSize + 2 /* for the messageType*/ ];

        if(data.Length > 512)
        {
            Debug.LogError("The net message is exceeding the 512 byte capacity. Message fragmentation is a feature to be added.");
        }

        BitStreamWriter writer = new BitStreamWriter(data);

        // message type
        writer.WriteUInt16(NetMessageFactory.GetNetMessageTypeId(message));

        message.NetSerialize(writer);
    }
}
