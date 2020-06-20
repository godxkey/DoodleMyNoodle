using Unity.Core;
using Unity.Entities;
using UnityX;

public struct SimulationOngoingTickId : IComponentData
{
    public SimulationOngoingTickId(uint tickId)
    {
        TickId = tickId;
    }

    public readonly uint TickId;
}

[AlwaysUpdateSystem]
public class UpdateSimulationTimeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (World is SimulationWorld simWorld)
        {
            Entity tickDataSingleton = simWorld.TickDataSingleton;

            // Increment tick id
            uint oldTickId = EntityManager.GetComponentData<SimulationOngoingTickId>(tickDataSingleton).TickId;
            uint newTickId = ValidateTickId(oldTickId + 1);
            EntityManager.SetComponentData(tickDataSingleton, new SimulationOngoingTickId(newTickId));

            // Set cached tick id
            simWorld.LatestTickId = newTickId;

            // Set time
            simWorld.CurrentFixTime = new FixTimeData(
                elapsedTime: SimulationConstants.TIME_STEP * (int)newTickId,
                deltaTime: SimulationConstants.TIME_STEP
            );
        }
    }

    private uint ValidateTickId(uint newOngoingTick)
    {
        if (World is SimulationWorld simWorld)
        {
            uint expectedTickId = simWorld.ExpectedNewTickId;

            if (newOngoingTick != expectedTickId)
            {
                Log.Error($"[{nameof(UpdateSimulationTimeSystem)}] " +
                    $"Our new tick '{newOngoingTick}' was not as expected: {expectedTickId}.");

                newOngoingTick = expectedTickId;
            }
        }

        return newOngoingTick;
    }
}
