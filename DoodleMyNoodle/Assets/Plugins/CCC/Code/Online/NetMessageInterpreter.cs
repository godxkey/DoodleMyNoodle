using CCC.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngineX;

public static class NetMessageInterpreter
{
#if DEBUG
    static LogChannel s_dataLogChannel = Log.CreateChannel("NetMessageByteData", activeByDefault: false);
#endif

    static BitStreamReader s_reader = new BitStreamReader(null);
    static BitStreamWriter s_writer = new BitStreamWriter(null);

    public static Type GetMessageType(byte[] messageData)
    {
        s_reader.SetNewBuffer(messageData);
        return NetSerializer.GetMessageType(s_reader);
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
#if DEBUG
        if (s_dataLogChannel.Active)
        {
            Log.Info(s_dataLogChannel, $"[{nameof(NetMessageInterpreter)}] Receive Message byte[{messageData.Length}]");
            DebugLogUtility.LogByteArray(s_dataLogChannel, messageData);
        }
#endif
        message = null;

        if (!ThreadUtility.IsMainThread) // need main thread because of static s_reader
        {
            Log.Error($"[{nameof(NetMessageInterpreter)}] Interpreting from thread not yet supported (but it should be!). ");
            return false;
        }

        s_reader.SetNewBuffer(messageData);


        if (messageData.Length < 2) // 2 minimum bytes required for the message type
        {
            Log.Error($"[{nameof(NetMessageInterpreter)}] Error interpreting: data size to small ({messageData.Length} bytes)");
            return false;
        }

        try
        {
            message = NetSerializer.Deserialize(s_reader);

            return true;
        }
        catch (Exception e)
        {
            Log.Error($"[{nameof(NetMessageInterpreter)}] Failed to deserialize Message. {e.Message} - {e.StackTrace}");
            return false;
        }
    }

#if DEBUG
    static uint s_avoidCollisionValue = 0;
#endif
    public static bool GetDataFromMessage<T>(in T message, out byte[] data, int byteLimit = int.MaxValue)
    {
        data = null;
        
        if (!ThreadUtility.IsMainThread) // need main thread because of static s_writer
        {
            Log.Error($"[{nameof(NetMessageInterpreter)}] Interpreting from thread not yet supported (but it should be!). ");
            return false;
        }

        int netBitSize = NetSerializer.GetSerializedBitSize(message);
        int messageSizeByte = netBitSize.CeiledToStep(8) / 8; // this will ceil the size to a multiple of 8

        if (messageSizeByte > byteLimit)
        {
            Log.Error($"The net message's ({data}) size ({messageSizeByte}) is exceeding the {byteLimit} byte capacity.");
            return false;
        }
        data = new byte[messageSizeByte];

        s_writer.SetNewBuffer(data);

        try
        {
            NetSerializer.Serialize(message, s_writer);
            
#if DEBUG
            if (s_dataLogChannel.Active)
            {
                Log.Info(s_dataLogChannel, $"[{nameof(NetMessageInterpreter)}] Serialize Message '{message}' to byte[{data.Length}]");
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
                    Log.Info(s_dataLogChannel, $"Data too long, logged to file {filePath}");
                }
            }
#endif
            return true;
        }
        catch (Exception e)
        {
            Log.Error($"[{nameof(NetMessageInterpreter)}] Failed to serialize Message'{message}'. {e.Message} - {e.StackTrace}");
            return false;
        }
    }
}
