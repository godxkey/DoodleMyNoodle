using CCC.Operations;
using System.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngineX;

namespace Sim.Operations
{
    public class SimDeserializationOperation : CoroutineOperation
    {
        public bool PartialSuccess;

        byte[] _serializedData;
        World _simulationWorld;

        internal SimDeserializationOperation(byte[] serializedData, World simulationWorld)
        {
            _serializedData = serializedData;
            _simulationWorld = simulationWorld;
        }

        World GetEntityWorldFromByteArray(byte[] bytes)
        {
            World tempWorld = new World("Deserialization temp world");
            unsafe
            {
                fixed (byte* dataPtr = bytes)
                {
                    using (MemoryBinaryReader reader = new MemoryBinaryReader(dataPtr))
                    {
                        SerializeUtility.DeserializeWorld(tempWorld.EntityManager.BeginExclusiveEntityTransaction(), reader);
                        tempWorld.EntityManager.EndExclusiveEntityTransaction();
                    }
                }
            }

            return tempWorld;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            Log.Info("Start Deserialization Process...");

            // force the 'ChangeDetectionSystem' to end the current sample, making our entity changes undetectable.
            var changeDetectionSystemEnd = _simulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
            if (changeDetectionSystemEnd != null)
            {
                changeDetectionSystemEnd.ForceEndSample();
            }

            World tempWorld = GetEntityWorldFromByteArray(_serializedData);
            _simulationWorld.EntityManager.CopyAndReplaceEntitiesFrom(tempWorld.EntityManager);
            tempWorld.Dispose();

            if (_simulationWorld is SimulationWorld simWorld)
            {
                simWorld.EntityClearAndReplaceCount++;
            }

            // force the 'ChangeDetectionSystem' to resample, making our entity changes undetectable.
            var changeDetectionSystemBegin = _simulationWorld.GetExistingSystem<ChangeDetectionSystemBegin>();
            if (changeDetectionSystemBegin != null)
            {
                changeDetectionSystemBegin.ResetSample();
            }

            TerminateWithSuccess();

            yield break;
        }
    }
}