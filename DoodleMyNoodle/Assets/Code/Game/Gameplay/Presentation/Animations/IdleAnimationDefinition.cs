using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Idle Animation")]
public class IdleAnimationDefinition : DOTWEENAnimationDefinition
{
    public float IdleHeight = 0.05f;

    public override Tween GetDOTWEENAnimationSequence(TriggerInput input)
    {
        Sequence sq = DOTween.Sequence();

        float idleStartY = 0;
        float idleEndY = idleStartY + IdleHeight;

        sq.Append(input.PresentationTarget.Bone.DOLocalMoveY(idleEndY, _duration / 2).SetEase(Ease.InOutQuad));
        sq.Append(input.PresentationTarget.Bone.DOLocalMoveY(idleStartY, _duration / 2).SetEase(Ease.InOutQuad));
        sq.SetLoops(-1);
        sq.Goto(Random.value * sq.Duration(includeLoops: false), andPlay: true);
        return sq;
    }
}