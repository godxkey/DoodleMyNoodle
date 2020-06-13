using CCC.Operations;
using Sim.Operations;
using System;
using System.Collections;
using Unity.Entities;

/// <summary>
/// This operation should be used for debugging purposes only
/// </summary>
public class SaveSimulationToMemoryOperation : CoroutineOperation
{
    public static byte[] s_SerializedSimulation;

    World _simulationWorld;

    public SaveSimulationToMemoryOperation(World simulationWorld)
    {
        this._simulationWorld = simulationWorld ?? throw new ArgumentNullException(nameof(simulationWorld));
    }

    protected override IEnumerator ExecuteRoutine()
    {
        SimSerializationOperationWithCache serializeOp = new SimSerializationOperationWithCache(_simulationWorld);

        yield return ExecuteSubOperationAndWaitForSuccess(serializeOp);

        s_SerializedSimulation = serializeOp.SerializationData;

        TerminateWithSuccess();
    }
}
