using System;
using UnityEngine;
using UnityEngineX;

public class FrozenSpriteChange : BindedPresentationEntityComponent
{
    public SpriteRenderer SpriteRenderer;
    public Color frozenColor;

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.HasComponent<Frozen>(SimEntity))
        {
            SpriteRenderer.color = frozenColor;
        }
        else
        {
            SpriteRenderer.color = Color.white;
        }
    }
}