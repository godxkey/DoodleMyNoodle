using SimulationControl;
using Unity.Entities;

[DisableAutoCreation]
public class EndViewSystem : ComponentSystem
{
    SimulationWorldSystem _worldMaster;

    protected override void OnCreate()
    {
        base.OnCreate();

        _worldMaster = World.GetOrCreateSystem<SimulationWorldSystem>();
    }

    protected override void OnUpdate()
    {
        World.EntityManager.CompleteAllJobs();
        _worldMaster.SimulationWorld.EntityManager.EndExclusiveEntityTransaction();
    }
}
