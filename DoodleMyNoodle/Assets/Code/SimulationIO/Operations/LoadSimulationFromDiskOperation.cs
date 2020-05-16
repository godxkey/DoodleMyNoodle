using CCC.Operations;
using System.Collections;
using Unity.Entities;

public class LoadSimulationFromDiskOperation : CoroutineOperation
{
    string _filePath;
    private World _simulationWorld;

    public LoadSimulationFromDiskOperation(string filePath, World simulationWorld)
    {
        _filePath = filePath;

        _simulationWorld = simulationWorld;
    }

    protected override IEnumerator ExecuteRoutine()
    {
        // get data
        ReadTextFromDiskOperation loadOp = new ReadTextFromDiskOperation(_filePath);
        yield return ExecuteSubOperationAndWaitForSuccess(loadOp);
        string serializedData = loadOp.TextResult;

        if (serializedData.IsNullOrEmpty())
        {
            TerminateWithAbnormalFailure($"Failed to load valid simulation from file {_filePath}");
            yield break;
        }

        // deserialize 
        yield return ExecuteSubOperationAndWaitForSuccess(SimulationView.DeserializeSimulation(serializedData, _simulationWorld));

        TerminateWithSuccess();
    }
}
