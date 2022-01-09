using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class PullEntitiesSystem : SimSystemBase
{
    private EntityCommandBufferSystem _ecbSytem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _ecbSytem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecbSytem.CreateCommandBuffer();

        fix currentTime = Time.ElapsedTime;

        Entities
            .WithAll<PhysicsVelocity>()
            .ForEach((Entity entity, ref PullData pullData, in FixTranslation position) =>
        {
            bool done = false;

            fix2 d = pullData.Destination - position;

            fix dLength = length(d);

            fix deltaTime = currentTime - pullData.StartTime;

            if ((dLength < (fix)0.25) || (deltaTime > (dLength * (fix)1.5)))
            {
                done = true;
            }
            else
            {
                fix2 dir = d / dLength;

                var newVelocity = new PhysicsVelocity(pullData.Speed * dir);

                SetComponent(entity, newVelocity);
            }

            if (done)
            {
                ecb.RemoveComponent<PullData>(entity);
            }
        }).Run();
    }
}

internal static partial class CommonWrites
{
    public static void RequestPull(ISimWorldReadWriteAccessor accessor, Entity entity, fix2 Destination, fix Speed)
    {
        accessor.SetOrAddComponent(entity, new PullData() 
        {
            Destination = Destination,
            Speed = Speed,
            StartTime = accessor.Time.ElapsedTime
        });
    }
}