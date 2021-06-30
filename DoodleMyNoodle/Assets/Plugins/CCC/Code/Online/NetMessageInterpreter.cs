using CCC.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.MathematicsX;
using UnityEngine;
using UnityEngineX;

public static class NetMessageInterpreter
{
#if DEBUG
    static LogChannel s_dataLogChannel = Log.CreateChannel("NetMessageByteData", activeByDefault: false);
#endif

    public static Type GetMessageType(byte[] messageData)
    {
        return NetSerializer.GetObjectType(messageData);
    }

    public static bool GetMessageFromData<T>(byte[] messageData, out T message)
    {
        if (GetMessageFromData(messageData, out object msg))
        {
            message = (T)msg;
            return message != null;
        }

        message = default;
        return false;
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
        try
        {
            message = NetSerializer.Deserialize(messageData);
            return true;
        }
        catch (Exception e)
        {
            message = null;
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

        try
        {
            data = NetSerializer.Serialize(message);
            
            if (data.Length > byteLimit)
            {
                Log.Error($"The net message's ({data}) size ({data.Length}) is exceeding the {byteLimit} byte capacity.");
                return false;
            }
#if DEBUG
            if (s_dataLogChannel.Active)
            {
                Log.Info(s_dataLogChannel, $"[{nameof(NetMessageInterpreter)}] Serialize Message '{message}' to byte[{data.Length}]");
                if (data.Length <= 128)
                    DebugLogUtility.LogByteArray(data);
                else
                {
                    string directory = Environment.CurrentDirectory;
                    if (Application.isEditor)
                        directory += "/..";

                    string filePath = $"{directory}/Logs/NetMessageMessageInterperter_{Time.time}_{message.GetType()}_{s_avoidCollisionValue++}.txt";
                    using (var writer = FileX.OpenFileFlushedAndReadyToWrite(filePath))
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
