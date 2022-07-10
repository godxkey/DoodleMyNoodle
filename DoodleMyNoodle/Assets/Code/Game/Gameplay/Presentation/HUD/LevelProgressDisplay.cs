using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class LevelProgressDisplay : GamePresentationBehaviour
{
    [SerializeField] private Slider _slider;

    public override void PresentationUpdate()
    {
        if (SimWorld.TryGetSingleton<SingletonCurrentLevelDefinition>(out var currentLevel))
        {
            if (SimWorld.TryGetBufferReadOnly<LevelDefinitionMobSpawn>(currentLevel, out var levelSpawns))
            {
                int totalMobsInLevel = Mathf.Max(levelSpawns.Length, 1); // to avoid div by 0

                // count the number of remaining enemies
                int remainingMobs = 0;
                var mobSpawnSingleton = SimWorld.GetSingletonEntity<SingletonElementRemainingLevelMobSpawnPoint>();
                if (mobSpawnSingleton != Entity.Null)
                {
                    var remainingMobSpawnsBuffer = SimWorld.GetBuffer<SingletonElementRemainingLevelMobSpawnPoint>(mobSpawnSingleton);
                    remainingMobs += remainingMobSpawnsBuffer.Length;
                }

                _slider.value = 1 - (remainingMobs / (float)totalMobsInLevel);
            }
            else if (SimWorld.TryGetComponent<LevelDefinitionDuration>(currentLevel, out var levelDuration))
            {
                var currentLevelStartTime = SimWorld.GetSingleton<SingletonCurrentLevelStartTime>();

                _slider.value = (float)((SimWorld.Time.ElapsedTime - currentLevelStartTime) / levelDuration);
            }
        }
    }
}