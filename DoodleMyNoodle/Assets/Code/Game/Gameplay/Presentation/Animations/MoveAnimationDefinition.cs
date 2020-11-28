using DG.Tweening;
using System;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Move Animation")]
public class MoveAnimationDefinition : AnimationDefinition
{
    public float WalkingHeight = 0.08f;

    private Sequence _currentSequence;

    public override void InteruptAnimation()
    {
        _currentSequence.Kill(true);
    }

    public override void TriggerAnimation(Vector3 spriteStartPos, Transform spriteTransform, AnimationData animationData)
    {
        _currentSequence = DOTween.Sequence();
        float walkingStartY = spriteStartPos.y;
        float walkingEndY = walkingStartY + WalkingHeight;
        _currentSequence.Append(spriteTransform.DOLocalMoveY(walkingEndY, (float)animationData.TotalDuration).SetEase(Ease.InOutQuad));
        _currentSequence.SetLoops(-1);
    }
}