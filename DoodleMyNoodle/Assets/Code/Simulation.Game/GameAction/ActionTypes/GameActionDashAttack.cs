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
            int2 instigatorTile = Helpers.GetTile(accessor.GetComponent<FixTranslation>(context.InstigatorPawn));

            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            if (!Pathfinding.FindNavigablePath(accessor, instigatorTile, paramTile.Tile, Pathfinding.MAX_PATH_LENGTH, path))
            {
                LogGameActionInfo(context, $"Discarding: cannot find navigable path from { instigatorTile} to { paramTile.Tile}.");
                return false;
            }

            // Get the last reachable point considering the user's AP
            int lastReachablePathPointIndex = Pathfinding.GetLastPathPointReachableWithinCost(path.AsArray().Slice(), settings.Range);

            // Remove unreachable points
            path.Resize(lastReachablePathPointIndex + 1, NativeArrayOptions.ClearMemory);

            NativeList<Entity> entityToDamage = new NativeList<Entity>(Allocator.Temp);

            fix2 min, max, center;
            fix radius = (fix)0.4;
            for (int i = 0; i < path.Length; i++)
            {
                int2 pos = path[i];
                if (!pos.Equals(instigatorTile))
                {
                    center = Helpers.GetTileCenter(path[i]);
                    min = center - fix2(radius, radius);
                    max = center + fix2(radius, radius);

                    CommonReads.Physics.OverlapAabb(accessor, min, max, entityToDamage, ignoreEntity: context.InstigatorPawn);
                }
            }
            entityToDamage.RemoveDuplicates();

            // set destination
            accessor.SetOrAddComponent(context.InstigatorPawn, new Destination() { Value = Helpers.GetTileCenter(path[path.Length - 1]) });

            CommonWrites.RequestDamage(accessor, context.InstigatorPawn, entityToDamage.AsArray(), settings.Damage);

            return true;
        }

        return false;
    }
}