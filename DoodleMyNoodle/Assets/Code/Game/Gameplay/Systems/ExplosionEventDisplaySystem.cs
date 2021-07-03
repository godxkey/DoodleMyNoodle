using System;
using UnityEngine;
using UnityEngineX;

public class ExplosionEventDisplaySystem : GamePresentationSystem<ExplosionEventDisplaySystem>
{
    public GameObject ExplosionPrefab;

    public override void OnPostSimulationTick()
    {
        var explosions = Cache.SimWorld.GetSingletonBufferReadOnly<EventExplosion>();

        for (int i = 0; i < explosions.Length; i++)
        {
            var explosionEvent = explosions[i];
            Vector2 position = (Vector2)explosionEvent.Position;
            var explosionVfx = Instantiate(ExplosionPrefab, position, Quaternion.identity);
            explosionVfx.transform.localScale = Vector3.one * ((float)explosionEvent.Radius * 2f);
        }
    }

    protected override void OnGamePresentationUpdate() { }
}