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
        PhysicsVelocity velocity = SimWorld.GetComponentData<PhysicsVelocity>(SimEntity);

        bool IsSpriteLookingRight = _baseSpriteIsLookingRight;
        if (SimWorld.HasComponent<DoodleStartDirection>(SimEntity))
        {
            IsSpriteLookingRight = SimWorld.GetComponentData<DoodleStartDirection>(SimEntity).IsLookingRight;
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