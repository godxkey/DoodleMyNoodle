using CCC.Operations;
using Sim.Operations;
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
        ReadBytesFromDiskOperation loadOp = new ReadBytesFromDiskOperation(_filePath);
        yield return ExecuteSubOperationAndWaitForSuccess(loadOp);
        byte[] serializedData = loadOp.Result;

        if (serializedData == null || serializedData.Length == 0)
        {
            TerminateWithAbnormalFailure($"Failed to load valid simulation from file {_filePath}");
            yield break;
        }

        // deserialize 
        yield return ExecuteSubOperationAndWaitForSuccess(new SimDeserializationOperation(serializedData, _simulationWorld));

        TerminateWithSuccess();
    }
}
