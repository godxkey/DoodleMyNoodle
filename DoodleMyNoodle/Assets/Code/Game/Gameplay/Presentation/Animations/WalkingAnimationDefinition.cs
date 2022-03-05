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

    public override Tween GetDOTWEENAnimationSequence(TriggerInput input)
    {
        Sequence sq = DOTween.Sequence();
        float walkingStartY = 0;
        float walkingEndY = walkingStartY + WalkingHeight;
        sq.Join(input.PresentationTarget.Bone.DOLocalMoveY(walkingEndY, _duration).SetEase(Ease.InOutQuad));
        sq.SetLoops(-1);
        return sq;
    }
}