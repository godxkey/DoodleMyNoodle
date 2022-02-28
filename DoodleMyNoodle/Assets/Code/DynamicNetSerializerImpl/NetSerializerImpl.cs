﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

[assembly: GenerateSerializer(typeof(int2))]
[assembly: GenerateSerializer(typeof(Vector2))]
[assembly: GenerateSerializer(typeof(Vector3))]
[assembly: GenerateSerializer(typeof(Vector4))]
[assembly: GenerateSerializer(typeof(fix))]
[assembly: GenerateSerializer(typeof(fix2))]
[assembly: GenerateSerializer(typeof(fix2x2))]
[assembly: GenerateSerializer(typeof(fix2x3))]
[assembly: GenerateSerializer(typeof(fix3))]
[assembly: GenerateSerializer(typeof(fix3x2))]
[assembly: GenerateSerializer(typeof(fix3x3))]
[assembly: GenerateSerializer(typeof(fix4))]
[assembly: GenerateSerializer(typeof(fix4x4))]
[assembly: GenerateSerializer(typeof(fixQuaternion))]

namespace Internals.OnlineServiceImpl
{
    public class NetSerializerImpl : INetSerializerImpl
    {

        // This is to setup the online service's factory. 
        // It's a little weird, but it's necessary because the OnlineService doesn't have access to this class directly
        static bool s_init = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeInitialize()
        {
            if (s_init)
                return;
            s_init = true;

            OnlineServicePhoton.FactoryCreator = () => new NetSerializerImpl();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static Dictionary<Type, ushort> s_typeToId = new Dictionary<Type, ushort>();
        static Dictionary<ushort, Type> s_idToType = new Dictionary<ushort, Type>();

        public NetSerializerImpl()
        {
            s_typeToId.Clear();
            s_idToType.Clear();

            // Net message -> id
            foreach (Type netMessageType in DynamicNetSerializationRegistry.types)
            {
                s_typeToId[netMessageType] = (ushort)s_typeToId.Count;
            }

            // id -> Net message
            foreach (KeyValuePair<Type, ushort> item in s_typeToId)
            {
                s_idToType.Add(item.Value, item.Key);
            }

            if (s_typeToId.Count > ushort.MaxValue)
            {
                Log.Error("To many NetMessage types for a UInt16.");
            }
        }

        ushort GetTypeId(object message)
        {
            if (Debug.isDebugBuild)
            {
                if (!s_typeToId.ContainsKey(message.GetType()))
                {
                    Log.Error("Cannot get typeId for netMessage of type " + message.GetType()
                        + ".  It has not been registered. Try re-running the registration code-gen");
                    return ushort.MaxValue;
                }
            }

            return s_typeToId[message.GetType()];
        }

        public bool IsValidType(Type type)
        {
            return s_typeToId.ContainsKey(type);
        }
        public bool IsValidType(ushort typeId)
        {
            return s_idToType.ContainsKey(typeId);
        }

        public int GetSerializedBitSize(object message)
        {
#if SAFETY
            try
            {
#endif

                return 16 + DynamicNetSerializationRegistry.map_GetBitSize[message.GetType()].Invoke(message);


#if SAFETY
            }
            catch (Exception e)
            {
                Log.Error($"[NetMessageFactoryImpl] " +
                    $"Failed to get message bit size from type [{message.GetType()}] : {e.Message} - {e.StackTrace}");
                return 0;
            }
#endif
        }

        public Type GetObjectType(BitStreamReader reader)
        {
#if SAFETY
            try
            {
#endif

                ushort typeId = reader.ReadUInt16();
                return s_idToType[typeId];


#if SAFETY
            }
            catch (Exception e)
            {
                Log.Error($"[NetMessageFactoryImpl]" +
                    $" Failed to GetMessageType : {e.Message} - {e.StackTrace}");
                return null;
            }
#endif
        }
        public void Serialize(object message, BitStreamWriter writer)
        {
#if SAFETY
            try
            {
#endif

                writer.WriteUInt16(GetTypeId(message));
                DynamicNetSerializationRegistry.map_Serialize[message.GetType()].Invoke(message, writer);


#if SAFETY
            }
            catch (Exception e)
            {
                Log.Error($"[NetMessageFactoryImpl]" +
                    $" Failed to serialize message of type [{message.GetType()}] : {e.Message} - {e.StackTrace}");
            }
#endif
        }

        public object Deserialize(BitStreamReader reader)
        {
#if SAFETY
            try
            {
#endif
                ushort typeId = reader.ReadUInt16();
                return DynamicNetSerializationRegistry.map_Deserialize[typeId].Invoke(reader);
#if SAFETY
            }
            catch (Exception e)
            {
                Log.Error($"[NetMessageFactoryImpl]" +
                    $" Failed to deserialize message : {e.Message} - {e.StackTrace}");
                return null;
            }
#endif
        }

        public ushort GetTypeId(Type type)
        {
            return s_typeToId[type];
        }

        public Type GetTypeFromId(ushort typeId)
        {
            return s_idToType[typeId];
        }
    }
}