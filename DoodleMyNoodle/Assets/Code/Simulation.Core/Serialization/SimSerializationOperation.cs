using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngineX;
using UnityX.EntitiesX.SerializationX;

namespace Sim.Operations
{
    // This operation launches the real 'SimSerializationOperation' and caches it.
    // If there is already one for the current tick id, it'll take the existing cached operation insteading of launching a new one
    public class SimSerializationOperationWithCache : CoroutineOperation
    {
        internal static SimSerializationOperation s_CachedSerializationOp;
        internal static uint s_CachedSerializationTickId = uint.MaxValue;

        public byte[] SerializationData;

        private World _world;

        internal SimSerializationOperationWithCache(World world)
        {
            _world = world;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            if (_world is SimulationWorld simWorld && simWorld.GetLastTickIdFromEntity() != s_CachedSerializationTickId)
            {
                s_CachedSerializationOp = new SimSerializationOperation(_world);
                s_CachedSerializationOp.Execute();
                s_CachedSerializationTickId = simWorld.GetLastTickIdFromEntity();
            }

            if (s_CachedSerializationOp.IsRunning)
            {
                yield return ExecuteSubOperationAndWaitForSuccess(s_CachedSerializationOp);
            }

            SerializationData = s_CachedSerializationOp.SerializationData;

            if (s_CachedSerializationOp.HasSucceeded)
                TerminateWithSuccess(s_CachedSerializationOp.Message);
            else
                TerminateWithAbnormalFailure(s_CachedSerializationOp.Message);
        }
    }

    public class SimSerializationOperation : CoroutineOperation
    {
        public interface IPtrObjectCollector
        {
            void BeginCollect(World world);
            void Collect(DynamicComponentTypeHandle typeHandle, ArchetypeChunk chunk, Dictionary<uint, byte[]> outResult);
            void EndCollect();
        }

        public static Dictionary<Type, IPtrObjectCollector> BlobAssetDataCollectors = new Dictionary<Type, IPtrObjectCollector>();

        public byte[] SerializationData;

        World _simulationWorld;

        internal SimSerializationOperation(World simulationWorld)
        {
            _simulationWorld = simulationWorld;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            Log.Info("Start Sim Serialization Process...");

            Dictionary<uint, byte[]> blobAssetsMap = new Dictionary<uint, byte[]>();
            Dictionary<ComponentType, Type> compToManaged = new Dictionary<ComponentType, Type>();
            Dictionary<ComponentType, DynamicComponentTypeHandle> compToHandle = new Dictionary<ComponentType, DynamicComponentTypeHandle>();
            NativeArray<ArchetypeChunk> chunks = _simulationWorld.EntityManager.GetAllChunks(Allocator.TempJob);

            foreach (var item in BlobAssetDataCollectors.Values)
            {
                item.BeginCollect(_simulationWorld);
            }

            // iterate over all chunks
            for (int i = 0; i < chunks.Length; i++)
            {
                ArchetypeChunk chunk = chunks[i];

                // iterate over all components in chunk
                foreach (ComponentType c in chunk.Archetype.GetComponentTypes())
                {
                    ComponentType componentType = c;
                    componentType.AccessModeType = ComponentType.AccessMode.ReadOnly;

                    // get managed type
                    if (!compToManaged.TryGetValue(componentType, out Type managedType))
                    {
                        managedType = componentType.GetManagedType();
                        compToManaged[componentType] = managedType;
                    }

                    // if collector exists for given component, invoke it
                    if (BlobAssetDataCollectors.TryGetValue(managedType, out IPtrObjectCollector collector))
                    {
                        // get componentTypeHandle (necessary for chunk data access)
                        if (!compToHandle.TryGetValue(componentType, out DynamicComponentTypeHandle typeHandle))
                        {
                            typeHandle = _simulationWorld.EntityManager.GetDynamicComponentTypeHandle(componentType);
                            compToHandle[componentType] = typeHandle;
                        }

                        // invoke!
                        collector.Collect(typeHandle, chunk, blobAssetsMap);
                    }
                }
            }
            chunks.Dispose();

            foreach (var item in BlobAssetDataCollectors.Values)
            {
                item.EndCollect();
            }

            SerializedWorld serializedWorld = new SerializedWorld();
            {
                serializedWorld.BlobAssets = new SerializedWorld.BlobAsset[blobAssetsMap.Count];
                int i = 0;
                foreach (var item in blobAssetsMap)
                {
                    serializedWorld.BlobAssets[i] = new SerializedWorld.BlobAsset()
                    {
                        Id = item.Key,
                        Data = item.Value,
                    };
                    i++;
                }
            }

            serializedWorld.WorldData = SerializeUtilityX.SerializeWorld(_simulationWorld.EntityManager, out object[] referencedObjects);

            SerializationData = NetSerializer.Serialize(serializedWorld);

            foreach (var obj in referencedObjects)
            {
                Log.Warning($"The ECS simulation references {obj}, which is a managed object. " +
                    $"This is not allowed for now due to serialization");
            }

            Log.Info("Sim Serialization Complete!");
            TerminateWithSuccess();
            yield break;
        }
    }
}