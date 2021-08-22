using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Death Animation")]
public class DeathAnimationDefinition : DOTWEENAnimationDefinition
{
    public override Tween GetDOTWEENAnimationSequence(Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        Sequence sq = DOTween.Sequence();
        sq.Join(spriteTransform.DOLocalRotate(new Vector3(0, 0, -90), Duration, RotateMode.LocalAxisAdd));
        sq.Join(spriteTransform.DOLocalMoveY(-0.5f, Duration));
        return sq;
    }
}