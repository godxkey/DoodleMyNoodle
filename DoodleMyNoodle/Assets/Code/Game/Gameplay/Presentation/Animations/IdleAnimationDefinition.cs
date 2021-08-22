using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Idle Animation")]
public class IdleAnimationDefinition : DOTWEENAnimationDefinition
{
    public float IdleHeight = 0.05f;

    public override Tween GetDOTWEENAnimationSequence(Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        Sequence sq = DOTween.Sequence();

        float idleStartY = spriteStartPos.y;
        float idleEndY = idleStartY + IdleHeight;

        sq.Append(spriteTransform.DOLocalMoveY(idleEndY, Duration / 2).SetEase(Ease.InOutQuad));
        sq.Append(spriteTransform.DOLocalMoveY(idleStartY, Duration / 2).SetEase(Ease.InOutQuad));
        sq.SetLoops(-1);
        sq.Goto(UnityEngine.Random.value * sq.Duration(includeLoops: false), andPlay: true);
        return sq;
    }
}