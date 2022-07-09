using System;
using UnityEngine;
using UnityEngineX;

public class ExplosionEventDisplaySystem : GamePresentationSystem<ExplosionEventDisplaySystem>
{
    public GameObject ExplosionPrefab;

    public override void PresentationPostSimulationTick()
    {
        var explosions = Cache.SimWorld.GetSingletonBufferReadOnly<SingletonEventElementExplosion>();

        for (int i = 0; i < explosions.Length; i++)
        {
            var explosionEvent = explosions[i];
            Vector2 position = (Vector2)explosionEvent.Position;
            var explosionVfx = Instantiate(ExplosionPrefab, position, Quaternion.identity);
            explosionVfx.transform.localScale = Vector3.one * ((float)explosionEvent.Radius * 2f);
        }
    }
}