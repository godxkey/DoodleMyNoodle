using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using System.Collections.Generic;
using UnityEngineX;

public struct RemainingLevelMobSpawnPoint : ISingletonBufferElementData, IComparable<RemainingLevelMobSpawnPoint>
{
    public Entity MobToSpawn;
    public MobSpawmModifierFlags Flags;
    public fix2 Position;

    public int CompareTo(RemainingLevelMobSpawnPoint other)
    {
        return Position.x.CompareTo(other.Position.x);
    }
}

public class SpawnMobsSystem : SimGameSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<RemainingLevelMobSpawnPoint>();
        RequireSingletonForUpdate<PlayerGroupDataTag>();
    }

    protected override void OnUpdate()
    {
        Entity playerGroupEntity = GetSingletonEntity<PlayerGroupDataTag>();
        FixTranslation playerGroupPosition = GetComponent<FixTranslation>(playerGroupEntity);
        DynamicBuffer<RemainingLevelMobSpawnPoint> mobSpawns = GetSingletonBuffer<RemainingLevelMobSpawnPoint>();

        using var _ = ListPool<RemainingLevelMobSpawnPoint>.Take(out var toSpawn);
        fix spawnXThreshold = playerGroupPosition.Value.x + SimulationGameConstants.EnemySpawnDistanceFromPlayerGroup;
        while (mobSpawns.Length > 0 && spawnXThreshold > mobSpawns[0].Position.x)
        {
            toSpawn.Add(mobSpawns[0]);
            mobSpawns.RemoveAt(0);
        }

        foreach (RemainingLevelMobSpawnPoint spawn in toSpawn)
        {
            var mobEntity = EntityManager.Instantiate(spawn.MobToSpawn);

            SetComponent<FixTranslation>(mobEntity, spawn.Position);

            if ((spawn.Flags & MobSpawmModifierFlags.Armored) != 0)
            {
                CommonWrites.AddStatusEffect(Accessor, new AddStatModifierRequest()
                {
                    Instigator = Entity.Null,
                    StackAmount = 1,
                    Target = mobEntity,
                    Type = StatModifierType.Armored
                });
            }

            if ((spawn.Flags & MobSpawmModifierFlags.Brutal) != 0)
            {
                CommonWrites.AddStatusEffect(Accessor, new AddStatModifierRequest()
                {
                    Instigator = Entity.Null,
                    StackAmount = 1,
                    Target = mobEntity,
                    Type = StatModifierType.Brutal
                });
            }

            if ((spawn.Flags & MobSpawmModifierFlags.Explosive) != 0)
            {
                EntityManager.AddComponentData(mobEntity, new ExplodeOnDeath()
                {
                    Damage = 4,
                    DestroyTiles = false,
                    HasExploded = false,
                    Radius = 1
                });
            }

            if ((spawn.Flags & MobSpawmModifierFlags.Fast) != 0)
            {
                CommonWrites.AddStatusEffect(Accessor, new AddStatModifierRequest()
                {
                    Instigator = Entity.Null,
                    StackAmount = 1,
                    Target = mobEntity,
                    Type = StatModifierType.Fast
                });
            }
        }
    }
}