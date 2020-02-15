using CCC.Operations;
using Sim.Operations;
using System.Collections;

public class SaveSimulationToDiskOperation : CoroutineOperation
{
    string _filePath;

    public SaveSimulationToDiskOperation(string filePath)
    {
        _filePath = filePath;
    }

    protected override IEnumerator ExecuteRoutine()
    {
        // Serialize sim
        SimSerializationOperationWithCache serializeOp = SimulationView.SerializeSimulation();
        yield return ExecuteSubOperationAndWaitForSuccess(serializeOp);
        string serializedData = serializeOp.SerializationData;

        // Save data to file
        yield return ExecuteSubOperationAndWaitForSuccess(new WriteTextToDiskOperation(serializedData, _filePath));

        TerminateWithSuccess($"Simulation sent to client through file: {_filePath}");
    }
}
