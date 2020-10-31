using System;
using UnityEngine;
using UnityEngineX;

public class ExplosionEventDisplaySystem : GamePresentationSystem<ExplosionEventDisplaySystem>
{
    public GameObject ExplosionPrefab;

    public override void OnPostSimulationTick()
    {
        Cache.SimWorld.Entities.ForEach((ref ExplosionEventData explosionData) =>
        {
            // TODO : Do a pool system for explosion
            Vector3 tileCenter = (Vector3)Helpers.GetTileCenter(explosionData.ExplodedTile);
            Instantiate(ExplosionPrefab, tileCenter, Quaternion.identity);
        });
    }

    protected override void OnGamePresentationUpdate() { }
}