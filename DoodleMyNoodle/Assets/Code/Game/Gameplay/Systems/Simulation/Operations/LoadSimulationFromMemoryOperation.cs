using System.Collections;
using CCC.Operations;

public class LoadSimulationFromMemoryOperation : CoroutineOperation
{
    protected override IEnumerator ExecuteRoutine()
    {
        // get data
        string serializedData = SaveSimulationToMemoryOperation.s_SerializedSimulation;
        if (serializedData.IsNullOrEmpty())
        {
            TerminateWithFailure("No valid simulation to load was found in memory");
            yield break;
        }

        // deserialize 
        yield return ExecuteSubOperationAndWaitForSuccess(SimulationView.DeserializeSimulation(serializedData));

        TerminateWithSuccess();
    }
}
