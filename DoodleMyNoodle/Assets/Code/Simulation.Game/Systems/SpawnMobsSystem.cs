using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using System.Collections.Generic;
using UnityEngineX;

public struct SingletonElementRemainingLevelMobSpawnPoint : ISingletonBufferElementData, IComparable<SingletonElementRemainingLevelMobSpawnPoint>
{
    public Entity MobToSpawn;
    public MobSpawmModifierFlags Flags;
    public fix2 Position;

    public int CompareTo(SingletonElementRemainingLevelMobSpawnPoint other)
    {
        return Position.x.CompareTo(other.Position.x);
    }
}

/// <summary>
/// This is to simulate the enemies moving even when unspawned
/// </summary>
public struct LevelUnspawnedMobsOffsetPositionSingleton : ISingletonComponentData
{
    public fix Value;

    public static implicit operator fix(LevelUnspawnedMobsOffsetPositionSingleton val) => val.Value;
    public static implicit operator LevelUnspawnedMobsOffsetPositionSingleton(fix val) => new LevelUnspawnedMobsOffsetPositionSingleton() { Value = val };
}

public class SpawnMobsSystem : SimGameSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<SingletonElementRemainingLevelMobSpawnPoint>();
        RequireSingletonForUpdate<PlayerGroupDataTag>();
    }

    protected override void OnUpdate()
    {
        // move offset (simulating the mobs moving, even when unspawned)
        var unspawnedMobsOffset = GetSingleton<LevelUnspawnedMobsOffsetPositionSingleton>();
        unspawnedMobsOffset.Value += (Time.DeltaTime * SimulationGameConstants.UnspawnedMobsMoveSpeed);
        SetSingleton(unspawnedMobsOffset);

        Entity playerGroupEntity = GetSingletonEntity<PlayerGroupDataTag>();
        FixTranslation playerGroupPosition = GetComponent<FixTranslation>(playerGroupEntity);
        DynamicBuffer<SingletonElementRemainingLevelMobSpawnPoint> mobSpawns = GetSingletonBuffer<SingletonElementRemainingLevelMobSpawnPoint>();

        using var _ = ListPool<SingletonElementRemainingLevelMobSpawnPoint>.Take(out var toSpawn);
        fix spawnXThreshold = playerGroupPosition.Value.x - unspawnedMobsOffset + SimulationGameConstants.EnemySpawnDistanceFromPlayerGroup;
        while (mobSpawns.Length > 0 && spawnXThreshold > mobSpawns[0].Position.x)
        {
            toSpawn.Add(mobSpawns[0]);
            mobSpawns.RemoveAt(0);
        }

        // spawn mobs
        foreach (SingletonElementRemainingLevelMobSpawnPoint spawn in toSpawn)
        {
            var mobEntity = EntityManager.Instantiate(spawn.MobToSpawn);

            SetComponent<FixTranslation>(mobEntity, spawn.Position + fix2(unspawnedMobsOffset, 0));

            if ((spawn.Flags & MobSpawmModifierFlags.Armored) != 0)
            {
                CommonWrites.AddStatusEffect(Accessor, new SystemRequestAddStatModifier()
                {
                    Instigator = Entity.Null,
                    StackAmount = 1,
                    Target = mobEntity,
                    Type = StatModifierType.Armored
                });
            }

            if ((spawn.Flags & MobSpawmModifierFlags.Brutal) != 0)
            {
                CommonWrites.AddStatusEffect(Accessor, new SystemRequestAddStatModifier()
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
                CommonWrites.AddStatusEffect(Accessor, new SystemRequestAddStatModifier()
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