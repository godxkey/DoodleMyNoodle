using DG.Tweening;
using System;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Death Animation")]
public class DeathAnimationDefinition : AnimationDefinition
{
    private Sequence _currentSequence;

    public override void InteruptAnimation()
    {
        _currentSequence.Kill(true);
    }

    public override void TriggerAnimation(Vector3 spriteStartPos, Transform spriteTransform, AnimationData animationData)
    {
        _currentSequence = DOTween.Sequence();
        _currentSequence.Join(spriteTransform.DOLocalRotate(new Vector3(0, 0, -90), 1, RotateMode.LocalAxisAdd));
        _currentSequence.Join(spriteTransform.DOLocalMoveY(-0.5f, 1));
    }
}