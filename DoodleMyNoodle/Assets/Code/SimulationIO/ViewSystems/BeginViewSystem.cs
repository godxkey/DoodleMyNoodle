using SimulationControl;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(ViewSystemGroup))]
public class BeginViewSystem : ComponentSystem
{
    [ReadOnly] public ExclusiveEntityTransaction ExclusiveSimWorld;

    SimulationWorldSystem _worldMaster;

    protected override void OnCreate()
    {
        base.OnCreate();

        _worldMaster = World.GetOrCreateSystem<SimulationWorldSystem>();
    }

    protected override void OnUpdate()
    {
        if (_worldMaster.SimulationWorld?.EntityManager == null)
            return;
        ExclusiveSimWorld = _worldMaster.SimulationWorld.EntityManager.BeginExclusiveEntityTransaction();
    }
}
