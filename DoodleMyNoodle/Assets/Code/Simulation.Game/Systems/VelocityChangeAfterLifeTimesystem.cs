using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct VelocityChangeAfterLifeTime : IComponentData
{
    public fix LifeTimeToTrigger;
    public fix2 VelocityMultiplier;
    public bool Applied;
}

public partial class VelocityChangeAfterLifeTimeSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity instigator, ref PhysicsVelocity velocity, ref VelocityChangeAfterLifeTime velocityChangeAfterLifeTime, in Lifetime lifetime) =>
        {
            if (!velocityChangeAfterLifeTime.Applied && lifetime.Value >= velocityChangeAfterLifeTime.LifeTimeToTrigger)
            {
                velocity.Linear *= velocityChangeAfterLifeTime.VelocityMultiplier;
                velocityChangeAfterLifeTime.Applied = true;
            }

        }).Schedule();
    }
}