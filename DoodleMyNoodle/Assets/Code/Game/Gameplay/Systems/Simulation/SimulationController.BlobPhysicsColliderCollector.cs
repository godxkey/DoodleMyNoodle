using CCC.Fix2D;
using Sim.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngineX;
using UnityX.EntitiesX.SerializationX;

public partial class SimulationController
{
    private class BlobPhysicsColliderCollector : SimSerializationOperation.IPtrObjectCollector
    {
        private ComponentTypeHandle<SimAssetId> _simAssetIdsHandle;
        private ComponentTypeHandle<TileColliderTag> _tileColliderTagHandle;
        private Dictionary<IntPtr, byte[]> _cachedColliders = new Dictionary<IntPtr, byte[]>();

        public void BeginCollect(World world)
        {
            _cachedColliders.Clear();
            _simAssetIdsHandle = world.EntityManager.GetComponentTypeHandle<SimAssetId>(isReadOnly: true);
            _tileColliderTagHandle = world.EntityManager.GetComponentTypeHandle<TileColliderTag>(isReadOnly: true);
        }

        public void Collect(DynamicComponentTypeHandle typeHandle, ArchetypeChunk chunk, Dictionary<uint, byte[]> outResult)
        {
            NativeArray<PhysicsColliderBlob> components = chunk.GetDynamicComponentDataArrayReinterpret<PhysicsColliderBlob>(typeHandle, PhysicsColliderBlob.TYPE_SIZE);
            NativeArray<SimAssetId> simAssetIds = chunk.Has(_simAssetIdsHandle) ? chunk.GetNativeArray(_simAssetIdsHandle) : default;

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].Collider.IsCreated)
                {
                    byte[] data;
                    IntPtr ptr;

                    unsafe
                    {
                        ptr = new IntPtr(components[i].Collider.GetUnsafePtr());
                    }

                    if (!_cachedColliders.TryGetValue(ptr, out data))
                    {
                        using (var writer = new MemoryBinaryWriter())
                        {
                            writer.Write(components[i].Collider);
                            data = writer.GetDataArray();
                        }

                        _cachedColliders[ptr] = data;
                    }

                    if (simAssetIds.IsCreated)
                    {
                        outResult[simAssetIds[i].Value] = data;
                    }
                    else
                    {
#if SAFETY && UNITY_EDITOR
                        if (!chunk.Has(_tileColliderTagHandle))
                        {
                            Log.Error($"Error in collider collection: An entity ({string.Join(", ", chunk.Archetype.GetComponentTypes().Select(c => c.ToString()))}) " +
                                $"has a collider but no SimAssetId");
                            continue;
                        }
#endif
                        outResult[ushort.MaxValue + 1] = data;
                    }
                }
            }
        }

        public void EndCollect()
        {
        }
    }
}
