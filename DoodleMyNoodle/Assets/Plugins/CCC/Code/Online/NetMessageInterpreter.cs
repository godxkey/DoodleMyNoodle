using CCC.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class NetMessageInterpreter
{
#if DEBUG_BUILD
    [ConfigVar(name: "log.netmessage_data", defaultValue: "0", ConfigVarFlag.Save, "Should we log the sent/received NetMessages.")]
    static ConfigVar s_logNetMessageData;
#endif

    static BitStreamReader s_reader = new BitStreamReader(null);
    static BitStreamWriter s_writer = new BitStreamWriter(null);

    public static Type GetMessageType(byte[] messageData)
    {
        s_reader.SetNewBuffer(messageData);
        return DynamicNetSerializer.GetMessageType(s_reader);
    }

    public static bool GetMessageFromData<T>(byte[] messageData, out T message)
    {
        bool res = GetMessageFromData(messageData, out object messageObj);

        if (res)
        {
            message = (T)messageObj;
        }
        else
        {
            message = default;
        }

        return res;
    }
    public static bool GetMessageFromData(byte[] messageData, out object message)
    {
#if DEBUG_BUILD
        if (s_logNetMessageData.BoolValue)
        {
            DebugService.Log($"[{nameof(NetMessageInterpreter)}] Receive Message byte[{messageData.Length}]");
            DebugLogUtility.LogByteArray(messageData);
        }
#endif
        message = null;

        if (!ThreadUtility.IsMainThread) // need main thread because of static s_reader
        {
            DebugService.LogError($"[{nameof(NetMessageInterpreter)}] Interpreting from thread not yet supported (but it should be!). ");
            return false;
        }

        s_reader.SetNewBuffer(messageData);


        if (messageData.Length < 2) // 2 minimum bytes required for the message type
        {
            DebugService.LogError($"[{nameof(NetMessageInterpreter)}] Error interpreting: data size to small ({messageData.Length} bytes)");
            return false;
        }

        try
        {
            message = DynamicNetSerializer.NetDeserialize(s_reader);

            return true;
        }
        catch (Exception e)
        {
            DebugService.LogError($"[{nameof(NetMessageInterpreter)}] Failed to deserialize Message. {e.Message} - {e.StackTrace}");
            return false;
        }
    }

#if DEBUG_BUILD
    static uint s_avoidCollisionValue = 0;
#endif
    public static bool GetDataFromMessage<T>(in T message, out byte[] data, int byteLimit = int.MaxValue)
    {
        data = null;
        
        if (!ThreadUtility.IsMainThread) // need main thread because of static s_writer
        {
            DebugService.LogError($"[{nameof(NetMessageInterpreter)}] Interpreting from thread not yet supported (but it should be!). ");
            return false;
        }

        int netBitSize = DynamicNetSerializer.GetNetBitSize(message);
        int messageSizeByte = netBitSize.CeiledToStep(8) / 8; // this will ceil the size to a multiple of 8

        if (messageSizeByte > byteLimit)
        {
            DebugService.LogError($"The net message's ({data}) size ({messageSizeByte}) is exceeding the {byteLimit} byte capacity.");
            return false;
        }
        data = new byte[messageSizeByte];

        s_writer.SetNewBuffer(data);

        try
        {
            DynamicNetSerializer.NetSerialize(message, s_writer);

#if DEBUG_BUILD
            if (s_logNetMessageData.BoolValue)
            {
                DebugService.Log($"[{nameof(NetMessageInterpreter)}] Serialize Message '{message}' to byte[{data.Length}]");
                if(data.Length <= 128)
                    DebugLogUtility.LogByteArray(data);
                else
                {
                    string directory = Environment.CurrentDirectory;
                    if (Application.isEditor)
                        directory += "/..";

                    string filePath = $"{directory}/Logs/NetMessageMessageInterperter_{Time.time}_{message.GetType()}_{s_avoidCollisionValue++}.txt";
                    using(var writer = FileX.OpenFileFlushedAndReadyToWrite(filePath))
                    {
                        DebugLogUtility.LogByteArrayToFile(data, writer.StreamWriter);
                    }
                    DebugService.Log($"Data too long, logged to file {filePath}");
                }
            }
#endif
            return true;
        }
        catch (Exception e)
        {
            DebugService.LogError($"[{nameof(NetMessageInterpreter)}] Failed to serialize Message'{message}'. {e.Message} - {e.StackTrace}");
            return false;
        }
    }
}
