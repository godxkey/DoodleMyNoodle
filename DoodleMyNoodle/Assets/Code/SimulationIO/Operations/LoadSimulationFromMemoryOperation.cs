using System;
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
        string serializedData = SaveSimulationToMemoryOperation.s_SerializedSimulation;
        if (string.IsNullOrEmpty(serializedData))
        {
            TerminateWithAbnormalFailure("No valid simulation to load was found in memory");
            yield break;
        }

        // deserialize 
        yield return ExecuteSubOperationAndWaitForSuccess(SimulationView.DeserializeSimulation(serializedData, _simulationWorld));

        TerminateWithSuccess();
    }
}
