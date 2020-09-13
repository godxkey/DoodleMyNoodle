using System;
using UnityEngine;
using UnityEngineX;

public class ExplosionEventDisplaySystem : GamePresentationBehaviour
{
    public GameObject ExplosionPrefab;

    public override void OnPostSimulationTick()
    {
        SimWorldCache.SimWorld.Entities.ForEach((ref ExplosionOnTileEventData explosionData) =>
        {
            // TODO : Do a pool system for explosion
            Vector3 tileCenter = (Vector3)Helpers.GetTileCenter(explosionData.ExplodedTile);
            Instantiate(ExplosionPrefab, tileCenter, Quaternion.identity);
        });
    }

    protected override void OnGamePresentationUpdate() { }
}