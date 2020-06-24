using CCC.Operations;
using Sim.Operations;
using System.Collections;
using Unity.Entities;

public class SaveSimulationToDiskOperation : CoroutineOperation
{
    string _filePath;
    private World _simulationWorld;

    public SaveSimulationToDiskOperation(string filePath, World simulationWorld)
    {
        _filePath = filePath;
        _simulationWorld = simulationWorld;
    }

    protected override IEnumerator ExecuteRoutine()
    {
        // Serialize sim
        SimSerializationOperationWithCache serializeOp = new SimSerializationOperationWithCache(_simulationWorld);
        yield return ExecuteSubOperationAndWaitForSuccess(serializeOp);
        byte[] serializedData = serializeOp.SerializationData;

        // Save data to file
        yield return ExecuteSubOperationAndWaitForSuccess(new WriteBytesToDiskOperation(serializedData, _filePath));

        TerminateWithSuccess($"Simulation saved to file: {_filePath}");
    }
}
