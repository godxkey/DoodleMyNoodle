using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Internals.OnlineServiceImpl
{
    public class DynamicNetSerializerImpl : IDynamicNetSerializerImpl
    {

        // This is to setup the online service's factory. 
        // It's a little weird, but it's necessary because the OnlineService doesn't have access to this class directly
        static bool init = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeInitialize()
        {
            if (init)
                return;
            init = true;

            OnlineServicePhoton.factoryCreator = () => new DynamicNetSerializerImpl();
        }




        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static Dictionary<Type, ushort> netMessageToId = new Dictionary<Type, ushort>();

        public DynamicNetSerializerImpl()
        {
            netMessageToId.Clear();

            // Net message -> id
            foreach (Type netMessageType in DynamicNetSerializationRegistry.types)
            {
                netMessageToId[netMessageType] = (ushort)netMessageToId.Count;
            }

            if (netMessageToId.Count > ushort.MaxValue)
            {
                DebugService.LogError("To many NetMessage types for a UInt16.");
            }
        }

        ushort GetTypeId(object message)
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

        public int GetNetBitSize(object message)
        {
            try
            {


                return 16 + DynamicNetSerializationRegistry.map_GetBitSize[message.GetType()].Invoke(message);


            }
            catch (Exception e)
            {
                DebugService.LogError("[NetMessageFactoryImpl] Failed to get message bit size from type [" + message.GetType() + "] : " + e.Message);
                return 0;
            }
        }

        public void NetSerialize(object message, BitStreamWriter writer)
        {
            try
            {

                writer.WriteUInt16(GetTypeId(message));
                DynamicNetSerializationRegistry.map_Serialize[message.GetType()].Invoke(message, writer);


            }
            catch (Exception e)
            {
                DebugService.LogError("[NetMessageFactoryImpl] Failed to serialize message of type [" + message.GetType() + "] : " + e.Message);
            }
        }

        public object NetDeserialize(BitStreamReader reader)
        {
            try
            {

                ushort typeId = reader.ReadUInt16();
                return DynamicNetSerializationRegistry.map_Deserialize[typeId].Invoke(reader);


            }
            catch (Exception e)
            {
                DebugService.LogError("[NetMessageFactoryImpl] Failed to deserialize message : " + e.Message);
                return null;
            }
        }
    }
}