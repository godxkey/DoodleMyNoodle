using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Move Animation")]
public class WalkingAnimationDefinition : DOTWEENAnimationDefinition
{
    public float WalkingHeight = 0.08f;

    public override Tween GetDOTWEENAnimationSequence(Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        Sequence sq = DOTween.Sequence();
        float walkingStartY = spriteStartPos.y;
        float walkingEndY = walkingStartY + WalkingHeight;
        sq.Join(spriteTransform.DOLocalMoveY(walkingEndY, Duration).SetEase(Ease.InOutQuad));
        sq.SetLoops(-1);
        return sq;
    }
}