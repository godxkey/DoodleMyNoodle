using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

// NB !!!
// I commented out the code in this system because:
// - we don't use it anymore
// - it has nested Entities.ForEach(..) which is not supported with Entities 0.50+ (it was never intented to be supported)
// - It's not effiencient. We should use Physics tests to find entities inside the portal radius, not position comparison

//public partial class NextLevelPortalSystem : SimGameSystemBase
//{
//    protected override void OnUpdate()
//    {
//        Entities
//        .ForEach((Entity nextLevelPortal, ref NextLevelPortalData nextLevelPortalData, in FixTranslation portalPos) =>
//        {
//            bool allPlayersInsidePortal = true;
//            fix2 portalPosition = portalPos.Value;
//            fix2 nextLevelPosition = nextLevelPortalData.NextLevelPos;
//            Entities
//            .ForEach((Entity playerController, in PersistentId playerID, in ControlledEntity pawnControlled) =>
//            {
//                if (allPlayersInsidePortal)
//                {
//                    fix2 pawnPosition = GetComponent<FixTranslation>(pawnControlled).Value;
//                    if ((pawnPosition.x > (portalPosition.x + (fix)0.5)) || (pawnPosition.x < (portalPosition.x - (fix)0.5))
//                    || (pawnPosition.y > (portalPosition.y + (fix)0.5)) || (pawnPosition.y < (portalPosition.y - (fix)0.5)))
//                    {
//                        allPlayersInsidePortal = false;
//                    }
//                }
//            })
//            .WithoutBurst()
//            .Run();

//            if (allPlayersInsidePortal)
//            {
//                Entities
//                .ForEach((Entity playerController, in PersistentId playerID, in ControlledEntity pawnControlled) =>
//                {
//                    SetComponent(pawnControlled.Value, new FixTranslation() { Value = nextLevelPosition });
//                })
//                .WithoutBurst()
//                .Run();
//            }
//        })
//        .WithoutBurst()
//        .Run();
//    }
//}