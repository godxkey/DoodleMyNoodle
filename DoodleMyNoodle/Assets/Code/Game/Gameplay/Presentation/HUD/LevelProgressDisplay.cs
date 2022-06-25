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
        if (SimWorld.TryGetSingleton<LevelMobCountSingleton>(out var totalMobsInLevel))
        {
            // count the number of remaining enemies
            int remainingMobs = SimWorld.Entities.WithAll<MobEnemyTag>().WithNone<DeadTag>().ToEntityQuery().CalculateEntityCount();

            var mobSpawnSingleton = SimWorld.GetSingletonEntity<RemainingLevelMobSpawnPoint>();
            if (mobSpawnSingleton != Entity.Null)
            {
                var remainingMobSpawnsBuffer = SimWorld.GetBufferReadOnly<RemainingLevelMobSpawnPoint>(mobSpawnSingleton);
                remainingMobs += remainingMobSpawnsBuffer.Length;
            }

            // avoid division by 0
            if (totalMobsInLevel.Value == 0)
            {
                totalMobsInLevel.Value = 1;
            }

            _slider.value = 1 - (remainingMobs / (float)totalMobsInLevel.Value);
        }
    }
}