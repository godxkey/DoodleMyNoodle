using Unity.Core;
using Unity.Entities;

struct SimulationOngoingTick : IComponentData
{
    public uint TickId;
}

public class UpdateSimulationTimeSystem : ComponentSystem
{
    SimulationOngoingTick TickDataSingleton
    {
        get
        {
            if (!HasSingleton<SimulationOngoingTick>())
            {
                EntityManager.CreateEntity(ComponentType.ReadWrite<SimulationOngoingTick>());
            }
            
            return GetSingleton<SimulationOngoingTick>();
        }
        set
        {
            SetSingleton(value);
        }
    }

    protected override void OnUpdate()
    {
        // Increment tick id
        var tickData = TickDataSingleton;
        ++tickData.TickId;
        TickDataSingleton = tickData;

        // Set time
        World.SetTime(new TimeData(
            elapsedTime: SimulationConstants.TIME_STEP_F * tickData.TickId,
            deltaTime: SimulationConstants.TIME_STEP_F
        ));
    }
}
