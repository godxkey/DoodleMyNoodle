using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
using UnityEngineX;
using UnityX.EntitiesX.SerializationX;

namespace Sim.Operations
{
    public class SimDeserializationOperation : CoroutineOperation
    {
        public interface IPtrObjectDistributor
        {
            void BeginDistribute(World world);
            void Distribute(DynamicComponentTypeHandle typeHandle, ArchetypeChunk chunk, Dictionary<uint, byte[]> objects);
            void EndDistribute();
        }

        public static Dictionary<Type, IPtrObjectDistributor> BlobAssetDataDistributors = new Dictionary<Type, IPtrObjectDistributor>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void StaticReset()
        {
            BlobAssetDataDistributors.Clear();
        }

        public bool PartialSuccess;

        byte[] _serializedData;
        World _simulationWorld;

        internal SimDeserializationOperation(byte[] serializedData, World simulationWorld)
        {
            _serializedData = serializedData;
            _simulationWorld = simulationWorld;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            Log.Info("Start Sim Deserialization Process...");

            SerializedWorld serializedWorld = NetSerializer.Deserialize<SerializedWorld>(_serializedData);

            SerializeUtilityX.DeserializeWorld(serializedWorld.WorldData, _simulationWorld.EntityManager);

            Dictionary<uint, byte[]> blobAssetsMap = new Dictionary<uint, byte[]>(serializedWorld.BlobAssets.Length);

            for (int i = 0; i < serializedWorld.BlobAssets.Length; i++)
            {
                blobAssetsMap.Add(serializedWorld.BlobAssets[i].Id, serializedWorld.BlobAssets[i].Data);
            }

            Dictionary<ComponentType, Type> compToManaged = new Dictionary<ComponentType, Type>();
            Dictionary<ComponentType, DynamicComponentTypeHandle> compToHandle = new Dictionary<ComponentType, DynamicComponentTypeHandle>();
            NativeArray<ArchetypeChunk> chunks = _simulationWorld.EntityManager.GetAllChunks(Allocator.TempJob);

            foreach (var item in BlobAssetDataDistributors.Values)
            {
                item.BeginDistribute(_simulationWorld);
            }

            // iterate over all chunks
            for (int i = 0; i < chunks.Length; i++)
            {
                ArchetypeChunk chunk = chunks[i];

                // iterate over all components in chunk
                foreach (ComponentType componentType in chunk.Archetype.GetComponentTypes())
                {
                    // get managed type
                    if (!compToManaged.TryGetValue(componentType, out Type managedType))
                    {
                        managedType = componentType.GetManagedType();
                        compToManaged[componentType] = managedType;
                    }

                    // if collector exists for given component, invoke it
                    if (BlobAssetDataDistributors.TryGetValue(managedType, out IPtrObjectDistributor collector))
                    {
                        // get componentTypeHandle (necessary for chunk data access)
                        if (!compToHandle.TryGetValue(componentType, out DynamicComponentTypeHandle typeHandle))
                        {
                            typeHandle = _simulationWorld.EntityManager.GetDynamicComponentTypeHandle(componentType);
                            compToHandle[componentType] = typeHandle;
                        }

                        // invoke!
                        collector.Distribute(typeHandle, chunk, blobAssetsMap);
                    }
                }
            }
            chunks.Dispose();

            foreach (var item in BlobAssetDataDistributors.Values)
            {
                item.EndDistribute();
            }

            Log.Info("Sim Deserialization Complete!");
            TerminateWithSuccess();

            yield break;
        }
    }
}