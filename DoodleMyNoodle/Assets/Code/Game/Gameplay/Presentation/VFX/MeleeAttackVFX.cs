using System;
using UnityEngine;
using UnityEngineX;
using DG.Tweening;

public class MeleeAttackVFX : GamePresentationBehaviour
{
    public Transform VFXTransform;
    public SpriteRenderer SpriteRenderer;

    public void StartMeleeAttackVFX(Sprite weaponSprite, float Duration, float Direction)
    {
        SpriteRenderer.sprite = weaponSprite;

        VFXTransform.DORotate(new Vector3(0, 0, 135 * Direction), Duration, RotateMode.LocalAxisAdd).OnComplete(()=> { Destroy(gameObject); });
    }
}