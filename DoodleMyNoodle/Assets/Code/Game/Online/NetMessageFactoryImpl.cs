using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Internals.OnlineServiceImpl
{
    public class NetMessageFactoryImpl : INetMessageFactory
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

        public ushort GetNetMessageTypeId(INetSerializable message)
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

        public INetSerializable CreateNetMessage(ushort messageType)
        {
            try
            {
                return NetMessageRegistry.factory.CreateValue(messageType);
            }
            catch
            {
                return null;
            }
        }
    }
}