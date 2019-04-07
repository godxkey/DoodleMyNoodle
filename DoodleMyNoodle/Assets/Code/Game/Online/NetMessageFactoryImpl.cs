using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Internals.OnlineServiceImpl
{
    public class NetMessageFactoryImpl : INetMessageFactoryImpl
    {
        static Dictionary<Type, ushort> netMessageToId = new Dictionary<Type, ushort>();

        public NetMessageFactoryImpl()
        {
            netMessageToId.Clear();

            // Net message -> id
            foreach (Type netMessageType in NetMessageRegistry.types)
            {
                netMessageToId[netMessageType] = (ushort)netMessageToId.Count;
            }

            if (netMessageToId.Count > ushort.MaxValue)
            {
                DebugService.LogError("To many NetMessage types for a UInt16.");
            }
        }

        public ushort GetNetMessageTypeId(object message)
        {
            if (Debug.isDebugBuild)
            {
                if (!netMessageToId.ContainsKey(message.GetType()))
                {
                    DebugService.LogError("Cannot get typeId for netMessage of type " + message.GetType()
                        + ".  It has not been registered. Try re-running the registration code-gen");
                    return ushort.MaxValue;
                }
            }

            return netMessageToId[message.GetType()];
        }

        public int GetMessageBitSize(ushort messageType, object message)
        {
            try
            {
                return NetMessageRegistry.netBitSizeMap[messageType].Invoke(message);
            }
            catch (Exception e)
            {
                DebugService.LogError("[NetMessageFactoryImpl] Failed to get message bit size from type [" + message.GetType() + "] : " + e.Message);
                return 0;
            }
        }

        public void SerializeMessage(ushort messageType, object message, BitStreamWriter writer)
        {
            try
            {
                NetMessageRegistry.serializationMap[messageType].Invoke(message, writer);
            }
            catch (Exception e)
            {
                DebugService.LogError("[NetMessageFactoryImpl] Failed to serialize message of type [" + message.GetType() + "] : " + e.Message);
            }
        }

        public object DeserializeMessage(ushort messageType, BitStreamReader reader)
        {
            try
            {
                return NetMessageRegistry.deserializationMap[messageType].Invoke(reader);
            }
            catch (Exception e)
            {
                DebugService.LogError("[NetMessageFactoryImpl] Failed to deserialize message of typeId [" + messageType + "] : " + e.Message);
                return null;
            }
        }
    }
}