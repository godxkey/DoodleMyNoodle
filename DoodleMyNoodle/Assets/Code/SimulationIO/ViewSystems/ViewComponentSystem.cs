using SimulationControl;
using Unity.Entities;

// fbessette: We should probably find a better pattern than this

public abstract class ViewComponentSystem : ComponentSystem
{
    protected ExternalSimWorldAccessor SimWorldAccessor => _simulationWorldSystem.SimWorldAccessor;
    private SimulationWorldSystem _simulationWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
    }
}

public abstract class ViewJobComponentSystem : JobComponentSystem
{
    protected ExternalSimWorldAccessor SimWorldAccessor => _simulationWorldSystem.SimWorldAccessor;
    private SimulationWorldSystem _simulationWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
    }
}

public abstract class ViewEntityCommandBufferSystem : EntityCommandBufferSystem
{
}
