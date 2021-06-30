using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
using UnityEngineX;

namespace UnityX.EntitiesX.SerializationX
{
    public static class SerializeUtilityX
    {
        public static void DeserializeWorld(byte[] bytes, EntityManager em, object[] unityObjects = null)
        {
            em.PrepareForDeserialize();
            unsafe
            {
                fixed (byte* dataPtr = bytes)
                {
                    using (MemoryBinaryReader reader = new MemoryBinaryReader(dataPtr))
                    {
                        SerializeUtility.DeserializeWorld(em.BeginExclusiveEntityTransaction(), reader, unityObjects);
                        em.EndExclusiveEntityTransaction();
                    }
                }
            }
        }

        public static byte[] SerializeWorld(EntityManager entityManager, out object[] referencedObjects)
        {
            using (var binaryWriter = new MemoryBinaryWriter())
            {
                SerializeUtility.SerializeWorld(entityManager, binaryWriter, out referencedObjects);

                return binaryWriter.GetDataArray();
            }
        }
    }

    public static class MemoryBinaryWriterExtension
    {
        public static byte[] GetDataArray(this MemoryBinaryWriter binaryWriter)
        {
            byte[] arr = new byte[binaryWriter.Length];

            unsafe
            {
                Marshal.Copy((IntPtr)binaryWriter.Data, arr, 0, binaryWriter.Length);
            }

            return arr;
        }
    }
}