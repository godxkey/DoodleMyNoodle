using CCC.Fix2D;
using Sim.Operations;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngineX;
using Collider = CCC.Fix2D.Collider;

public partial class SimulationController
{
    private class BlobPhysicsColliderDistributor : SimDeserializationOperation.IPtrObjectDistributor
    {
        private ComponentTypeHandle<SimAssetId> _simAssetIdsHandle;
        private Dictionary<uint, BlobAssetReference<Collider>> _cachedColliders = new Dictionary<uint, BlobAssetReference<Collider>>();

        public void BeginDistribute(World world)
        {
            _cachedColliders.Clear();
            _simAssetIdsHandle = world.EntityManager.GetComponentTypeHandle<SimAssetId>(isReadOnly: true);
        }

        public void Distribute(DynamicComponentTypeHandle typeHandle, ArchetypeChunk chunk, Dictionary<uint, byte[]> objects)
        {
            NativeArray<PhysicsColliderBlob> components = chunk.GetDynamicComponentDataArrayReinterpret<PhysicsColliderBlob>(typeHandle, PhysicsColliderBlob.TYPE_SIZE);
            NativeArray<SimAssetId> simAssetIds = chunk.Has(_simAssetIdsHandle) ? chunk.GetNativeArray(_simAssetIdsHandle) : default;

            for (int i = 0; i < components.Length; i++)
            {
                uint id = ushort.MaxValue + 1;

                if (simAssetIds.IsCreated)
                {
                    id = simAssetIds[i].Value;
                }

                if (objects.TryGetValue(id, out byte[] data))
                {
                    // try get cached collider
                    if (!_cachedColliders.TryGetValue(id, out BlobAssetReference<Collider> collider))
                    {
                        unsafe
                        {
                            fixed (byte* dataPtr = data)
                            {
                                using (MemoryBinaryReader reader = new MemoryBinaryReader(dataPtr, data.Length))
                                {
                                    try
                                    {
                                        collider = reader.Read<Collider>();
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Exception(e);
                                        collider = default;
                                    }
                                    _cachedColliders[id] = collider;
                                }
                            }
                        }
                    }

                    components[i] = new PhysicsColliderBlob() { Collider = collider };
                }
            }
        }

        public void EndDistribute()
        {
        }
    }
}
