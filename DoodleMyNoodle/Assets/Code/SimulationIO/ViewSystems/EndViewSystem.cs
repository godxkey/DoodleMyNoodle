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
        if (_worldMaster.SimulationWorld?.EntityManager == null)
            return;

        World.EntityManager.CompleteAllJobs();
        //_worldMaster.SimulationWorld.EntityManager.EndExclusiveEntityTransaction();
        //DebugService.Log("EndExclusiveEntityTransaction");
    }
}
