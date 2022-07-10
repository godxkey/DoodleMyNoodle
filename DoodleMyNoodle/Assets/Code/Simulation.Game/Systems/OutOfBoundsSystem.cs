using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;

public partial class OutOfBoundsSystem : SimGameSystemBase
{
    private EntityCommandBufferSystem _ecbSytem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _ecbSytem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (TryGetSingletonEntity<PlayerGroupDataTag>(out Entity playerGroupEntity))
        {
            FixTranslation playerGroupTranslation = GetComponent<FixTranslation>(playerGroupEntity);

            // Characters out of bounds
            Entities
                .ForEach((Entity entity, ref FixTranslation translation, ref RemainingPeriodicActionCount remainingPeriodicActionCount, in PeriodicActionCount actionCount, in Team team) =>
                {
                    // ennemy npc exited screen on the left
                    if (team.Value == 1 && translation.Value.x <= (playerGroupTranslation.Value.x - SimulationGameConstants.OutOfBoundsLeftDistanceFromPlayerGroup))
                    {
                        translation.Value = fix2(translation.Value.x + SimulationGameConstants.OutOfBoundsLeftDistanceFromPlayerGroup + SimulationGameConstants.OutOfBoundsRightDistanceFromPlayerGroup, translation.Value.y);
                        remainingPeriodicActionCount.Value = actionCount.Value;
                    }
                }).Schedule();

            // Projectiles out of bounds
            var ecb = _ecbSytem.CreateCommandBuffer();
            Entities
                .ForEach((Entity entity, ref FixTranslation translation, in ProjectileTag projectileTag) =>
                {
                    // projectile exited screen on the right
                    if (translation.Value.x >= (playerGroupTranslation.Value.x + SimulationGameConstants.OutOfBoundsRightDistanceFromPlayerGroup))
                    {
                        ecb.DestroyEntity(entity);
                    }
                }).Schedule();
            _ecbSytem.AddJobHandleForProducer(Dependency);
        }
    }
}