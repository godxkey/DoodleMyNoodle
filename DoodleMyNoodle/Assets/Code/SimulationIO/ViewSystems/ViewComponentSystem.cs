using SimulationControl;
using Unity.Entities;

[UpdateInGroup(typeof(ViewSystemGroup))]
public abstract class ViewComponentSystem : ComponentSystem
{
    protected SimWorldAccessor SimWorldAccessor => _simulationWorldSystem.SimWorldAccessor;
    private SimulationWorldSystem _simulationWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
    }
}

[UpdateInGroup(typeof(ViewSystemGroup))]
public abstract class ViewJobComponentSystem : JobComponentSystem
{
    protected SimWorldAccessor SimWorldAccessor => _simulationWorldSystem.SimWorldAccessor;
    private SimulationWorldSystem _simulationWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
    }
}
