using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class NextLevelPortalSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .ForEach((Entity nextLevelPortal, ref NextLevelPortalData nextLevelPortalData, in FixTranslation portalPos) =>
        {
            bool allPlayersInsidePortal = true;
            fix2 portalPosition = portalPos.Value;
            fix2 nextLevelPosition = nextLevelPortalData.NextLevelPos;
            Entities
            .ForEach((Entity playerController, in PersistentId playerID, in ControlledEntity pawnControlled) =>
            {
                if (allPlayersInsidePortal)
                {
                    fix2 pawnPosition = GetComponent<FixTranslation>(pawnControlled).Value;
                    if ((pawnPosition.x > (portalPosition.x + (fix)0.5)) || (pawnPosition.x < (portalPosition.x - (fix)0.5))
                    || (pawnPosition.y > (portalPosition.y + (fix)0.5)) || (pawnPosition.y < (portalPosition.y - (fix)0.5)))
                    {
                        allPlayersInsidePortal = false;
                    }
                }
            })
            .WithoutBurst()
            .Run();

            if (allPlayersInsidePortal)
            {
                Entities
                .ForEach((Entity playerController, in PersistentId playerID, in ControlledEntity pawnControlled) =>
                {
                    SetComponent(pawnControlled.Value, new FixTranslation() { Value = nextLevelPosition });
                })
                .WithoutBurst()
                .Run();
            }
        })
        .WithoutBurst()
        .Run();
    }
}