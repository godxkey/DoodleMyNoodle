using CCC.Fix2D;
using Unity.Entities;

public struct RemainingLifetime : IComponentData
{
    public fix Value;

    public static implicit operator fix(RemainingLifetime val) => val.Value;
    public static implicit operator RemainingLifetime(fix val) => new RemainingLifetime() { Value = val };
}

public struct Lifetime : IComponentData
{
    public fix Value;

    public static implicit operator fix(Lifetime val) => val.Value;
    public static implicit operator Lifetime(fix val) => new Lifetime() { Value = val };
}

public partial class UpdateLifetimeSystem : SimGameSystemBase
{
    private EntityCommandBufferSystem _ecbSytem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _ecbSytem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        fix deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Lifetime lifetime) =>
        {
            lifetime.Value += deltaTime;
        }).Schedule();

        var ecb = _ecbSytem.CreateCommandBuffer();
        Entities.ForEach((Entity entity, ref RemainingLifetime remainingLifetime) =>
        {
            remainingLifetime.Value -= deltaTime;
            if (remainingLifetime.Value < 0)
            {
                ecb.DestroyEntity(entity);
            }
        }).Schedule();
        _ecbSytem.AddJobHandleForProducer(Dependency);
    }
}
