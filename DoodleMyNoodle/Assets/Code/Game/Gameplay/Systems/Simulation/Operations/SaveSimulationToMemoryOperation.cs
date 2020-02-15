using CCC.Operations;
using Sim.Operations;
using System.Collections;

/// <summary>
/// This operation should be used for debugging purposes only
/// </summary>
public class SaveSimulationToMemoryOperation : CoroutineOperation
{
    public static string s_SerializedSimulation;

    protected override IEnumerator ExecuteRoutine()
    {
        SimSerializationOperation serializeOp = SimulationView.SerializeSimulation();

        yield return ExecuteSubOperationAndWaitForSuccess(serializeOp);

        s_SerializedSimulation = serializeOp.SerializationData;

        TerminateWithSuccess();
    }
}
