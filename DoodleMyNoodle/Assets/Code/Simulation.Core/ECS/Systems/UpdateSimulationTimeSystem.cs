using Unity.Core;
using Unity.Entities;

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
    protected Entity TickDataSingleton
    {
        get
        {
            if (_tickSingletonQuery.IsEmptyIgnoreFilter)
            {
#if UNITY_EDITOR
                var entity = EntityManager.CreateEntity(typeof(SimulationOngoingTickId));
                EntityManager.SetName(entity, "WorldTick");
#else
                EntityManager.CreateEntity(typeof(SimulationOngoingTickId));
#endif
            }

            return _tickSingletonQuery.GetSingletonEntity();
        }
    }

    private EntityQuery _tickSingletonQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _tickSingletonQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<SimulationOngoingTickId>());
    }

    protected override void OnUpdate()
    {
        if (World is SimulationWorld simWorld)
        {
            // Increment tick id
            uint oldTickId = EntityManager.GetComponentData<SimulationOngoingTickId>(TickDataSingleton).TickId;
            uint newTickId = ValidateTickId(oldTickId + 1);
            EntityManager.SetComponentData(TickDataSingleton, new SimulationOngoingTickId(newTickId));

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
                DebugService.LogError($"[{nameof(UpdateSimulationTimeSystem)}] " +
                    $"Our new tick '{newOngoingTick}' was not as expected: {expectedTickId}.");

                newOngoingTick = expectedTickId;
            }
        }

        return newOngoingTick;
    }
}
