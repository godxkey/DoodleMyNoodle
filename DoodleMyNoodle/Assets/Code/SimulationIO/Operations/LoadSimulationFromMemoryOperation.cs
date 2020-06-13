using System;
using Sim.Operations;
using System.Collections;
using CCC.Operations;
using Unity.Entities;

public class LoadSimulationFromMemoryOperation : CoroutineOperation
{
    World _simulationWorld;

    public LoadSimulationFromMemoryOperation(World simulationWorld)
    {
        this._simulationWorld = simulationWorld ?? throw new ArgumentNullException(nameof(simulationWorld));
    }

    protected override IEnumerator ExecuteRoutine()
    {
        // get data
        byte[] serializedData = SaveSimulationToMemoryOperation.s_SerializedSimulation;
        if (serializedData == null || serializedData.Length == 1)
        {
            TerminateWithAbnormalFailure("No valid simulation to load was found in memory");
            yield break;
        }

        // deserialize 
        yield return ExecuteSubOperationAndWaitForSuccess(new SimDeserializationOperation(serializedData, _simulationWorld));

        TerminateWithSuccess();
    }
}
