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
public struct NextLevelPlaylistEntry : ISingletonBufferElementData
{
    public Entity LevelDefinition;
}

public struct LevelToAddToPlaylist : IBufferElementData
{
    public Entity LevelDefinition;
}

public class GameFlowSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        AddLevelsToPlaylistIfNeeded();

        StartGameIfNeeded();

        if (HasSingleton<GameStartedTag>())
        {
            ChangeLevelOrWinIfNeeded();
            GameOverIfPlayersDead();
        }
    }

    private void AddLevelsToPlaylistIfNeeded()
    {
        var levelsToAddQuery = GetEntityQuery(typeof(LevelToAddToPlaylist));
        if (levelsToAddQuery.IsEmpty) 
            return;

        var nextLevels = GetSingletonBuffer<NextLevelPlaylistEntry>();
        Entities.ForEach((DynamicBuffer<LevelToAddToPlaylist> levelsToAdd) =>
        {
            foreach (var item in levelsToAdd)
            {
                nextLevels.Add(new NextLevelPlaylistEntry() { LevelDefinition = item.LevelDefinition });
            }
        }).Run();

        EntityManager.DestroyEntity(levelsToAddQuery);
    }

    private void StartGameIfNeeded()
    {
        if (HasSingleton<GameStartedTag>())
            return;

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

        if (atLeastOnePlayerExists && everyoneIsReady)
        {
            CreateSingleton<GameStartedTag>();
        }
    }

    private void GameOverIfPlayersDead()
    {
        if (TryGetSingletonEntity<PlayerGroupDataTag>(out Entity playerGroup))
        {
            // Team HP is empty, they're dead = they lost :(
            if (EntityManager.TryGetComponent(playerGroup, out Health health))
            {
                if (health.Value <= 0)
                {
                    // Ennemy win !
                    if (!HasSingleton<WinningTeam>())
                    {
                        CreateSingleton(new WinningTeam { Value = (int)DesignerFriendlyTeam.Baddies });
                    }
                }
            }

            //// Team reached their destination for this level = they win !
            //if (EntityManager.TryGetComponent(playerGroup, out FixTranslation translation))
            //{
            //    if (TryGetSingleton(out GameOverDestinationToReachSingleton gameOverDestinationToReachSingleton))
            //    {
            //        if (translation.Value.x >= gameOverDestinationToReachSingleton.XPosition)
            //        {
            //        }
            //    }
            //}
        }
    }

    private void ChangeLevelOrWinIfNeeded()
    {
        // any mobs alive?
        var enemiesQuery = GetEntityQuery(ComponentType.Exclude<Prefab>(), typeof(MobEnemyTag));
        if (!enemiesQuery.IsEmpty)
            return;

        // any remaining mobs to spawn ?
        var remainingEnemyMobs = GetSingletonBuffer<RemainingLevelMobSpawnPoint>();
        if (!remainingEnemyMobs.IsEmpty)
            return;

        // any remaining levels ?
        var nextLevels = GetSingletonBuffer<NextLevelPlaylistEntry>();
        if (nextLevels.IsEmpty)
        {
            // Player wins !
            if (!HasSingleton<WinningTeam>())
            {
                CreateSingleton(new WinningTeam { Value = (int)DesignerFriendlyTeam.Player });
            }
            return;
        }

        // start next level!
        var levelToStart = nextLevels[0];
        nextLevels.RemoveAt(0);
        StartLevel(levelToStart.LevelDefinition);
    }

    private void StartLevel(Entity levelDefinition)
    {
        if (!HasSingleton<PlayerGroupDataTag>())
        {
            Log.Error("No player group. Cannot start level.");
            return;
        }

        if (!EntityManager.HasComponent<LevelDefinitionMobSpawn>(levelDefinition))
        {
            Log.Error($"LevelDefinition {EntityManager.GetNameSafe(levelDefinition)} has no {nameof(LevelDefinitionMobSpawn)} component. Cannot start level.");
            return;
        }

        var playerGroup = GetSingletonEntity<PlayerGroupDataTag>();

        // reset player group position
        {
            var playerGroupPos = GetComponent<FixTranslation>(playerGroup);
            SetComponent<FixTranslation>(playerGroup, fix2(0, playerGroupPos.Value.y));
        }

        // Set mob spawns
        {
            var remainingLevelSpawns = GetSingletonBuffer<RemainingLevelMobSpawnPoint>();
            var levelDefinitonSpawns = GetBuffer<LevelDefinitionMobSpawn>(levelDefinition);
            remainingLevelSpawns.Clear();
            foreach (var item in levelDefinitonSpawns)
            {
                remainingLevelSpawns.Add(new RemainingLevelMobSpawnPoint()
                {
                    Flags = item.Flags,
                    MobToSpawn = item.MobToSpawn,
                    Position = item.Position
                });
            }
            remainingLevelSpawns.AsNativeArray().Sort();
        }
    }
}