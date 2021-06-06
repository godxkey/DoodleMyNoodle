using CCC.Operations;
using Sim.Operations;
using System.Collections;
using Unity.Entities;
using UnityEngineX;

public class RepackSimulationOperation : CoroutineOperation
{
    private World _currentWorld;
    public World NewWorld { get; private set; }

    public RepackSimulationOperation(World currentWorld, World newWorld)
    {
        _currentWorld = currentWorld;
        NewWorld = newWorld;
    }

    protected override IEnumerator ExecuteRoutine()
    {
        Log.Info("repack begin: " + ((SimulationWorld)NewWorld).GetLastTickIdFromEntity());

        // Serialize sim
        SimSerializationOperationWithCache serializeOp = new SimSerializationOperationWithCache(_currentWorld);
        yield return ExecuteSubOperationAndWaitForSuccess(serializeOp);

        // deserialize sim
        SimDeserializationOperation deserializeOp = new SimDeserializationOperation(serializeOp.SerializationData, NewWorld);
        yield return ExecuteSubOperationAndWaitForSuccess(deserializeOp);

        Log.Info("repack end: " + ((SimulationWorld)NewWorld).GetLastTickIdFromEntity());
        // Terminate
        TerminateWithSuccess($"Simulation world packed into new world.");
    }
}