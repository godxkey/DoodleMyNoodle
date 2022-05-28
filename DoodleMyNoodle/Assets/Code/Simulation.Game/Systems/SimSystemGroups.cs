using Unity.Entities;

[UpdateAfter(typeof(CCC.Fix2D.PhysicsSystemGroup))]
public class PostPhysicsSystemGroup : SimComponentSystemGroup
{
    private CCC.Fix2D.EndFramePhysicsSystem _endPhysics;
    private CCC.Fix2D.PhysicsWorldSystem _physicsWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _endPhysics = World.GetExistingSystem<CCC.Fix2D.EndFramePhysicsSystem>();
        _physicsWorldSystem = World.GetExistingSystem<CCC.Fix2D.PhysicsWorldSystem>();
    }

    protected override void OnUpdate()
    {
        _endPhysics.FinalJobHandle.Complete();
        _physicsWorldSystem.PhysicsWorldFullyUpdated = true;

        base.OnUpdate();
    }
}

[UpdateAfter(typeof(PostPhysicsSystemGroup))] public class InputSystemGroup : SimComponentSystemGroup { }

[UpdateAfter(typeof(InputSystemGroup))] public class SignalSystemGroup : SimComponentSystemGroup { }
[UpdateAfter(typeof(InputSystemGroup))] public class MovementSystemGroup : SimComponentSystemGroup { }

