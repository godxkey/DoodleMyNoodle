using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Sprite Animation")]
public class SpriteChangeAnimationDefinition : AnimationDefinition
{
    public Sprite InitialSprite;
    public Sprite NewSprite;

    public override void FinishAnimation(Entity entity, Transform spriteTransform)
    {
        SpriteRenderer spriteRenderer = GetSpriteRenderer(spriteTransform);
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = InitialSprite;
        }
    }

    public override void StopAnimation(Entity entity, Transform spriteTransform)
    {
        SpriteRenderer spriteRenderer = GetSpriteRenderer(spriteTransform);
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = InitialSprite;
        }
    }

    protected override void OnTriggerAnimation(Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        SpriteRenderer spriteRenderer = GetSpriteRenderer(spriteTransform);
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = NewSprite;
        }
    }

    private SpriteRenderer GetSpriteRenderer(Transform spriteTransform) 
    {
        return spriteTransform.gameObject.GetComponentInChildren<SpriteRenderer>();
    }
}