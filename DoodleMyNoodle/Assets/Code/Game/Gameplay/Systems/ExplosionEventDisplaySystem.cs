using System;
using UnityEngine;
using UnityEngineX;

public class ExplosionEventDisplaySystem : GamePresentationSystem<ExplosionEventDisplaySystem>
{
    public GameObject ExplosionPrefab;

    public override void OnPostSimulationTick()
    {
        Cache.SimWorld.Entities.ForEach((ref ExplosionEventData explosionEvent) =>
        {
            Vector2 position = (Vector2)explosionEvent.Position;
            var explosion = Instantiate(ExplosionPrefab, position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * ((float)explosionEvent.Radius * 2f);
        });
    }

    protected override void OnGamePresentationUpdate() { }
}