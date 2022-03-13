using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class SlowStatusEffectSpriteChange : BindedPresentationEntityComponent
{
    public SpriteRenderer SpriteRenderer;
    public Color frozenColor;

    protected override void OnGamePresentationUpdate()
    {
        bool isSlowed = false;
        if(SimWorld.TryGetBufferReadOnly(SimEntity, out DynamicBuffer<StatusEffect> StatusEffects)) 
        {
            foreach (var statusEffect in StatusEffects)
            {
                if (statusEffect.Type == StatusEffectType.Slow)
                {
                    isSlowed = true;
                    break;
                }
            }
        }

        if (isSlowed)
        {
            SpriteRenderer.color = frozenColor;
        }
        else
        {
            SpriteRenderer.color = Color.white;
        }
    }
}