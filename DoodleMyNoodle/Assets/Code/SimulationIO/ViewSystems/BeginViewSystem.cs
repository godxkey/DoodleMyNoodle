using SimulationControl;
using Unity.Collections;
using Unity.Entities;

//[DisableAutoCreation]
//public class BeginViewSystem : ComponentSystem
//{
//    //[ReadOnly] public ExclusiveEntityTransaction ExclusiveSimWorld;

//    SimulationWorldSystem _simWorldSystem;

//    protected override void OnCreate()
//    {
//        base.OnCreate();

//        _simWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
//        _simWorldSystem.SimWorldAccessor.BeginViewSystem = this;
//    }

//    protected override void OnDestroy()
//    {
//        base.OnDestroy();

//        if(_simWorldSystem?.SimWorldAccessor != null)
//            _simWorldSystem.SimWorldAccessor.BeginViewSystem = null;
//    }

//    protected override void OnUpdate()
//    {
//        if (_simWorldSystem.SimulationWorld?.EntityManager == null)
//            return;
//        //ExclusiveSimWorld = _simWorldSystem.SimulationWorld.EntityManager.BeginExclusiveEntityTransaction();
//        //DebugService.Log("BeginExclusiveEntityTransaction");
//    }
//}
