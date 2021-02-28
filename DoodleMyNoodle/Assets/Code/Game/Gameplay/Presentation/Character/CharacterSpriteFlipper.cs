using CCC.Fix2D;
using System;
using UnityEngine;
using UnityEngineX;

public class CharacterSpriteFlipper : BindedPresentationEntityComponent
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private bool _baseSpriteIsLookingRight = true;

    protected override void OnGamePresentationUpdate()
    {
        PhysicsVelocity velocity = GamePresentationCache.Instance.SimWorld.GetComponentData<PhysicsVelocity>(SimEntity);

        bool IsSpriteLookingRight = _baseSpriteIsLookingRight;
        if (GamePresentationCache.Instance.SimWorld.HasComponent<DoodleStartDirection>(SimEntity))
        {
            IsSpriteLookingRight = GamePresentationCache.Instance.SimWorld.GetComponentData<DoodleStartDirection>(SimEntity).IsLookingRight;
        }

        if (velocity.Linear.x > 0)
        {
            _spriteRenderer.flipX = !IsSpriteLookingRight;
        }
        else if (velocity.Linear.x < 0)
        {
            _spriteRenderer.flipX = IsSpriteLookingRight;
        }
    }
}