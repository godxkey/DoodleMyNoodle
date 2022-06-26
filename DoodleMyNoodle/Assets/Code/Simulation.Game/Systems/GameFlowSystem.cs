using CCC.Fix2D;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public struct GameStartedTag : IComponentData { }

public struct ReadyToPlay : IComponentData
{
    public bool Value;

    public static implicit operator bool(ReadyToPlay val) => val.Value;
    public static implicit operator ReadyToPlay(bool val) => new ReadyToPlay() { Value = val };
}

/// <summary>
/// Identify the next level to play when the current one is finished/
/// </summary>
public struct SingletonElementNextLevelPlaylist : ISingletonBufferElementData
{
    public Entity LevelDefinition;
}

public struct SingletonCurrentLevelDefinition : ISingletonComponentData
{
    public Entity Value;

    public static implicit operator Entity(SingletonCurrentLevelDefinition val) => val.Value;
    public static implicit operator SingletonCurrentLevelDefinition(Entity val) => new SingletonCurrentLevelDefinition() { Value = val };
}

public struct SingletonCurrentLevelStartTime : ISingletonComponentData
{
    public fix Value;

    public static implicit operator fix(SingletonCurrentLevelStartTime val) => val.Value;
    public static implicit operator SingletonCurrentLevelStartTime(fix val) => new SingletonCurrentLevelStartTime() { Value = val };
}

public struct LevelToAddToPlaylist : IBufferElementData
{
    public Entity LevelDefinition;
}

public struct SingletonRequestNextLevel : IComponentData { }

public class GameFlowSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        AddLevelsToPlaylistIfNeeded();

        if (!HasSingleton<GameStartedTag>())
        {
            if (CanStartGame())
                CreateSingleton<GameStartedTag>();
        }
        else if (!HasSingleton<WinningTeam>())
        {
            if (HavePlayersLost())
            {
                CreateSingleton(new WinningTeam { Value = (int)DesignerFriendlyTeam.Baddies });
                Log.Info($"[GameFlowSystem] Players lose!");
            }
            else if (ShouldChangeLevel())
            {
                // any remaining levels ?
                var nextLevels = GetSingletonBuffer<SingletonElementNextLevelPlaylist>();
                if (nextLevels.IsEmpty)
                {
                    CreateSingleton(new WinningTeam { Value = (int)DesignerFriendlyTeam.Player });
                    Log.Info($"[GameFlowSystem] Players win!.");
                }
                else
                {
                    // start next level
                    var levelToStart = nextLevels[0];
                    nextLevels.RemoveAt(0);
                    StartLevel(levelToStart.LevelDefinition);
                    Log.Info($"[GameFlowSystem] Changing level to {EntityManager.GetNameSafe(levelToStart.LevelDefinition)}.");
                }
            }
        }
    }

    private void AddLevelsToPlaylistIfNeeded()
    {
        var levelsToAddQuery = GetEntityQuery(typeof(LevelToAddToPlaylist));
        if (levelsToAddQuery.IsEmpty)
            return;

        var nextLevels = GetSingletonBuffer<SingletonElementNextLevelPlaylist>();
        Entities.ForEach((DynamicBuffer<LevelToAddToPlaylist> levelsToAdd) =>
        {
            foreach (var item in levelsToAdd)
            {
                nextLevels.Add(new SingletonElementNextLevelPlaylist() { LevelDefinition = item.LevelDefinition });
            }
        }).Run();

        EntityManager.DestroyEntity(levelsToAddQuery);
    }

    private bool CanStartGame()
    {
        bool everyoneIsReady = true;
        bool atLeastOnePlayerExists = false;

        // check if every player is ready
        Entities
            .ForEach((in Active active, in ReadyToPlay readyToPlay, in ControlledEntity pawn) =>
            {
                if (active)
                {
                    atLeastOnePlayerExists = true;

                    if (!readyToPlay && HasComponent<Controllable>(pawn))
                    {
                        everyoneIsReady = false; // if a team member is NOT ready
                    }
                }
            }).Run();
        return everyoneIsReady && atLeastOnePlayerExists && HasSingleton<PlayerGroupDataTag>();
    }

    private bool HavePlayersLost()
    {
        if (TryGetSingletonEntity<PlayerGroupDataTag>(out Entity playerGroup))
        {
            // Team HP is empty, they're dead = they lost :(
            if (EntityManager.TryGetComponent(playerGroup, out Health health))
            {
                if (health.Value <= 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool ShouldChangeLevel()
    {
        Entity currentLevel = GetSingleton<SingletonCurrentLevelDefinition>();

        if (HasComponent<LevelDefinitionDuration>(currentLevel))
        {
            fix levelStartTime = GetSingleton<SingletonCurrentLevelStartTime>();
            fix levelDuration = GetComponent<LevelDefinitionDuration>(currentLevel);
            if (Time.ElapsedTime - levelStartTime > levelDuration)
            {
                Log.Info($"[GameFlowSystem] Should change level because duration exceeded.");
                return true;
            }
        }

        // If we're in a combat level, check that everyone is dead
        if (currentLevel == Entity.Null || EntityManager.HasComponent<LevelDefinitionMobSpawn>(currentLevel))
        {
            // no mobs alive?
            var enemiesQuery = GetEntityQuery(ComponentType.Exclude<Prefab>(), typeof(MobEnemyTag));
            if (enemiesQuery.IsEmpty)
            {
                // no remaining mobs to spawn ?
                var remainingEnemyMobs = GetSingletonBuffer<SingletonElementRemainingLevelMobSpawnPoint>();
                if (remainingEnemyMobs.IsEmpty)
                {
                    Log.Info($"[GameFlowSystem] Should change level because all enemies dead.");
                    return true;
                }
            }
        }

        if (HasSingleton<SingletonRequestNextLevel>())
        {
            DestroySingleton<SingletonRequestNextLevel>();
            Log.Info($"[GameFlowSystem] Should change level because of cheat request.");
            return true;
        }

        return false;
    }

    private void StartLevel(Entity levelDefinition)
    {
        if (!HasSingleton<PlayerGroupDataTag>())
        {
            Log.Error("No player group. Cannot start level.");
            return;
        }

        var playerGroup = GetSingletonEntity<PlayerGroupDataTag>();

        // reset player group position
        {
            var playerGroupPos = GetComponent<FixTranslation>(playerGroup);
            SetComponent<FixTranslation>(playerGroup, fix2(0, playerGroupPos.Value.y));
        }

        // restore hp
        {
            var maxHP = GetComponent<MaximumFix<Health>>(playerGroup);
            SetComponent<Health>(playerGroup, maxHP.Value);
        }

        // restore item charges
        {
            Entities.ForEach((ref ItemCharges charges, in ItemStatingCharges startingCharges) =>
            {
                charges.Value = startingCharges;
            }).Run();
        }
        // fbessette: should we remove game effects ? What about game effects that need to stay across levels like passives?

        // destroy linguering mobs
        EntityManager.DestroyEntity(GetEntityQuery(typeof(MobEnemyTag)));

        // clear previous spawns
        var remainingLevelSpawns = GetSingletonBuffer<SingletonElementRemainingLevelMobSpawnPoint>();
        remainingLevelSpawns.Clear();

        // Set mob spawns
        if (EntityManager.HasComponent<LevelDefinitionMobSpawn>(levelDefinition))
        {
            var levelDefinitonSpawns = GetBuffer<LevelDefinitionMobSpawn>(levelDefinition);
            foreach (var item in levelDefinitonSpawns)
            {
                remainingLevelSpawns.Add(new SingletonElementRemainingLevelMobSpawnPoint()
                {
                    Flags = item.Flags,
                    MobToSpawn = item.MobToSpawn,
                    Position = item.Position
                });
            }
            remainingLevelSpawns.AsNativeArray().Sort();

            // reset their offset (used to simulate them walking even when unspawned)
            SetSingleton<LevelUnspawnedMobsOffsetPositionSingleton>(fix.Zero);
        }

        SetSingleton<SingletonCurrentLevelStartTime>(Time.ElapsedTime);
        SetSingleton<SingletonCurrentLevelDefinition>(levelDefinition);
    }
}