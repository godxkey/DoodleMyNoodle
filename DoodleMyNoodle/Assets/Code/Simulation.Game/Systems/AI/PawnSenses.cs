#if UNITY_EDITOR
#define GIZMOS
#endif

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;

public partial class CommonReads
{
    public static class PawnSenses
    {
        public static fix SIGHT_RANGE => 10;
        public static fix2 PAWN_EYES_OFFSET => fix2(0, fix(0.15));

        public static void FindAllPawnsInSight(ISimWorldReadAccessor accessor, EntityQuery pawnsQuery, Entity pawn, Entity pawnController, NativeList<Entity> result, bool withGizmos = false)
        {
            Team excludeTeam = Team.Null;

            if (pawnController != Entity.Null && accessor.TryGetComponent(pawnController, out Team team))
            {
                excludeTeam = team;
            }

            FindAllPawnsInSightInternal(accessor, pawn, pawnsQuery, excludeTeam, result, withGizmos);
        }

        public static void FindAllPawnsInSight(ISimWorldReadAccessor accessor, EntityQuery pawnsQuery, Entity pawn, Team excludeTeam, NativeList<Entity> result, bool withGizmos = false)
        {
            FindAllPawnsInSightInternal(accessor, pawn, pawnsQuery, excludeTeam, result, withGizmos);
        }

        private static void FindAllPawnsInSightInternal(
            ISimWorldReadAccessor accessor,
            Entity pawn,
            EntityQuery pawnsQuery,
            Team excludeTeam,
            NativeList<Entity> result,
            bool drawGizmos = false)
        {
            fix2 pawnPos = accessor.GetComponent<FixTranslation>(pawn);
            fix2 pawnEyes = pawnPos + UpdateArcherAISystem.PAWN_EYES_OFFSET;

            var positions = accessor.GetComponentDataFromEntity<FixTranslation>();
            var attackableEntities = pawnsQuery.ToEntityArray(Allocator.TempJob);

            fix detectRangeSq = UpdateArcherAISystem.DETECT_RANGE * UpdateArcherAISystem.DETECT_RANGE;

            foreach (var enemy in attackableEntities)
            {
                // excluse self
                if (enemy == pawn) 
                    continue;

                Entity enemyController = CommonReads.GetPawnController(accessor, enemy);

                // excluse teammates
                if (enemyController != Entity.Null && 
                    accessor.TryGetComponent(enemyController, out Team enemyTeam) && 
                    enemyTeam == excludeTeam) 
                    continue;

                fix2 enemyPos = (fix2)positions[enemy].Value;

                // excluse enemes too far
                if (lengthsq(enemyPos - pawnEyes) > detectRangeSq) // not too far
                {
                    continue;
                }

                // excluse enemies behind walls
                if (TilePhysics.RaycastTerrain(accessor, pawnEyes, enemyPos)) // line of sight
                {
#if GIZMOS
                    if (drawGizmos)
                    {
                        UnityEngine.Gizmos.color = UnityEngine.Color.gray;
                        UnityEngine.Gizmos.DrawLine((UnityEngine.Vector3)(fix3)pawnEyes, (UnityEngine.Vector3)(fix3)enemyPos);
                    }
#endif
                    continue;
                }

                result.Add(enemy);

#if GIZMOS
                if (drawGizmos)
                {
                    UnityEngine.Gizmos.color = UnityEngine.Color.white;
                    UnityEngine.Gizmos.DrawLine((UnityEngine.Vector3)(fix3)pawnEyes, (UnityEngine.Vector3)(fix3)enemyPos);
                }
#endif
            }

            attackableEntities.Dispose();
        }
    }
}
