using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public class GameActionDashAttack : GameAction<GameActionDashAttack.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public int Range;
        public int Damage;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                Damage = Damage,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public int Range;
        public int Damage;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description(settings.Range)
            {
                IncludeSelf = false,
                MustBeReachable = true
            }
        };

        return useContract;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData, Settings settings)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            fix2 instigatorPos = accessor.GetComponent<FixTranslation>(context.InstigatorPawn);
            int2 instigatorTile = Helpers.GetTile(instigatorPos);

            Pathfinding.PathResult path = new Pathfinding.PathResult(Allocator.Temp);

            var pathfindingContext = new Pathfinding.Context(CommonReads.GetTileWorld(accessor));
            pathfindingContext.AgentCapabilities.Drop1TileCost = fix.MaxValue;
            pathfindingContext.AgentCapabilities.Jump1TileCost = fix.MaxValue;

            if (!Pathfinding.FindNavigablePath(pathfindingContext, instigatorPos, Helpers.GetTileCenter(paramTile.Tile), Pathfinding.AgentCapabilities.DefaultMaxCost, ref path))
            {
                LogGameActionInfo(context, $"Discarding: cannot find navigable path from { instigatorTile} to { paramTile.Tile}.");
                return false;
            }

            // Get the last reachable point considering the user's AP
            int lastReachablePathPointIndex = path.LastReachableSegmentWithinCost(settings.Range);

            // Remove unreachable points
            path.Segments.Resize(lastReachablePathPointIndex + 1, NativeArrayOptions.ClearMemory);

            NativeList<Entity> entityToDamage = new NativeList<Entity>(Allocator.Temp);

            fix2 min, max, center;
            fix radius = (fix)0.4;
            for (int i = 0; i < path.Segments.Length; i++)
            {
                fix2 segmentPos = path.Segments[i].EndPosition;
                if (!Helpers.GetTile(segmentPos).Equals(instigatorTile))
                {
                    center = segmentPos;
                    min = center - fix2(radius, radius);
                    max = center + fix2(radius, radius);

                    CommonReads.Physics.OverlapAabb(accessor, min, max, entityToDamage, ignoreEntity: context.InstigatorPawn);
                }
            }
            entityToDamage.RemoveDuplicates();

            // set destination
            accessor.SetOrAddComponent(context.InstigatorPawn, new Destination() { Value = path.Segments[path.Segments.Length - 1].EndPosition });

            CommonWrites.RequestDamage(accessor, context.InstigatorPawn, entityToDamage.AsArray(), settings.Damage);

            return true;
        }

        return false;
    }
}