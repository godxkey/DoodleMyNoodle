using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Attack Animation")]
public class AttackAnimationDefinition : AnimationDefinition
{
    private Sequence _currentSequence;

    public override void InteruptAnimation()
    {
        _currentSequence.Kill(true);
    }

    public override void TriggerAnimation(Vector3 spriteStartPos, Transform spriteTransform, AnimationData animationData)
    {
        Vector3 startPos = spriteStartPos;
        Vector3 endPos = new Vector3(startPos.x + animationData.Direction.x, startPos.y + animationData.Direction.y, startPos.z);
        _currentSequence.Append(spriteTransform.DOLocalMove(endPos, (float)animationData.TotalDuration / 2));
        _currentSequence.Append(spriteTransform.DOLocalMove(startPos, (float)animationData.TotalDuration / 2));
    }
}