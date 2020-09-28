using CCC.Operations;
using System.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngineX;
using UnityX.EntitiesX.SerializationX;

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

        protected override IEnumerator ExecuteRoutine()
        {
            Log.Info("Start Sim Deserialization Process...");

            //// force the 'ChangeDetectionSystem' to end the current sample, making our entity changes undetectable.
            //var changeDetectionSystemEnd = _simulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
            //if (changeDetectionSystemEnd != null)
            //{
            //    changeDetectionSystemEnd.ForceEndSample();
            //}

            //World tempWorld = new World("tempWorld");
            //GetEntityWorldFromByteArray(_serializedData, tempWorld);
            //_simulationWorld.EntityManager.CopyAndReplaceEntitiesFrom(tempWorld.EntityManager);
            //tempWorld.Dispose();

            //if (_simulationWorld is SimulationWorld simWorld)
            //{
            //    simWorld.OnEndDeserialization();
            //}

            //// force the 'ChangeDetectionSystem' to resample, making our entity changes undetectable.
            //var changeDetectionSystemBegin = _simulationWorld.GetExistingSystem<ChangeDetectionSystemBegin>();
            //if (changeDetectionSystemBegin != null)
            //{
            //    changeDetectionSystemBegin.ResetSample();
            //}

            SerializeUtilityX.DeserializeWorld(_serializedData, _simulationWorld.EntityManager);

            Log.Info("Sim Deserialization Complete!");
            TerminateWithSuccess();

            yield break;
        }
    }
}