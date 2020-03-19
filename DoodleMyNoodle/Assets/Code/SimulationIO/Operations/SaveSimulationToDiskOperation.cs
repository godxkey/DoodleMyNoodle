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
        SimSerializationOperationWithCache serializeOp = SimulationView.SerializeSimulation(_simulationWorld);
        yield return ExecuteSubOperationAndWaitForSuccess(serializeOp);
        string serializedData = serializeOp.SerializationData;

        // Save data to file
        yield return ExecuteSubOperationAndWaitForSuccess(new WriteTextToDiskOperation(serializedData, _filePath));

        TerminateWithSuccess($"Simulation sent to client through file: {_filePath}");
    }
}
