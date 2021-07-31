using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class PortalTeleportationSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .ForEach((Entity portalEntity, in Portal portalData, in FixTranslation portalPos) =>
        {
            fix2 portalPosition = portalPos.Value;
            fix2 portalNextPosition = portalData.NextPos;

            Entities
            .ForEach((Entity entity, ref FixTranslation entityPos) =>
            {
                if (!HasComponent<Portal>(entity))
                {
                    fix2 pawnPosition = entityPos.Value;
                    if ((pawnPosition.x < (portalPosition.x + (fix)0.5)) && (pawnPosition.x > (portalPosition.x - (fix)0.5))
                    && (pawnPosition.y < (portalPosition.y + (fix)0.5)) && (pawnPosition.y > (portalPosition.y - (fix)0.5)))
                    {
                        if (!HasComponent<InsidePortalTag>(entity))
                        {
                            CommonWrites.RequestTeleport(Accessor, entity, portalNextPosition, true);
                        }
                    }
                    else
                    {
                        if (HasComponent<InsidePortalTag>(entity))
                        {
                            EntityManager.RemoveComponent<InsidePortalTag>(entity);
                        }
                    }
                }
            })
            .WithoutBurst()
            .WithStructuralChanges()
            .Run();
        })
        .WithoutBurst()
        .Run();
    }
}