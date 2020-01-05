using CCC.Operations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class LoadSimulationFromDiskOperation : CoroutineOperation
{
    string _filePath;


    public LoadSimulationFromDiskOperation(string filePath)
    {
        _filePath = filePath;
    }

    protected override IEnumerator ExecuteRoutine()
    {
        // get data
        ReadTextFromDiskOperation loadOp = new ReadTextFromDiskOperation(_filePath);
        yield return ExecuteSubOperationAndWaitForSuccess(loadOp);
        string serializedData = loadOp.TextResult;

        if (serializedData.IsNullOrEmpty())
        {
            TerminateWithFailure($"Failed to load valid simulation from file {_filePath}");
            yield break;
        }

        // deserialize 
        yield return ExecuteSubOperationAndWaitForSuccess(SimulationView.DeserializeSimulation(serializedData));

        TerminateWithSuccess();
    }
}
