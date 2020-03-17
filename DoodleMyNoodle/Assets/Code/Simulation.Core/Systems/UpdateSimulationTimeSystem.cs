using Unity.Core;
using Unity.Entities;

public struct SimulationOngoingTickId : IComponentData
{
    public uint TickId;
}

[AlwaysUpdateSystem]
public class UpdateSimulationTimeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Increment tick id
        var tickDataSingleton = ((SimulationWorld)World).TickDataSingleton;
        var tickData = EntityManager.GetComponentData<SimulationOngoingTickId>(tickDataSingleton);
        ++tickData.TickId;

        ValidateTickId(ref tickData);

        EntityManager.SetComponentData<SimulationOngoingTickId>(tickDataSingleton, tickData);

        // Set time
        World.SetTime(new TimeData(
            elapsedTime: SimulationConstants.TIME_STEP_F * tickData.TickId,
            deltaTime: SimulationConstants.TIME_STEP_F
        ));
    }


    private void ValidateTickId(ref SimulationOngoingTickId newOngoingTick)
    {
        if (World is SimulationWorld simWorld)
        {
            uint expectedTickId = simWorld.ExpectedNewTickId;

            if (newOngoingTick.TickId != expectedTickId)
            {
                DebugService.LogError($"[{nameof(UpdateSimulationTimeSystem)}] " +
                    $"Our new tick '{newOngoingTick.TickId}' was not as expected: {expectedTickId}.");
                
                newOngoingTick.TickId = expectedTickId;
            }
        }
    }
}
