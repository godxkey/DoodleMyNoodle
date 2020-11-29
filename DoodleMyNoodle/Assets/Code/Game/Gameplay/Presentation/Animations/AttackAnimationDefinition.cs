using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Entities;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Attack Animation")]
public class AttackAnimationDefinition : AnimationDefinition
{
    private Dictionary<Entity, Sequence> _sequences;

    public override void InteruptAnimation(Entity entity)
    {
        if (_sequences.ContainsKey(entity))
        {
            _sequences[entity].Kill(true);
        }
    }

    public override void TriggerAnimation(Entity entity, Vector3 spriteStartPos, Transform spriteTransform, AnimationData animationData)
    {
        Sequence _currentSequence = DOTween.Sequence();
        Vector3 startPos = spriteStartPos;
        Vector3 endPos = new Vector3(startPos.x + animationData.Direction.x, startPos.y + animationData.Direction.y, startPos.z);
        _currentSequence.Append(spriteTransform.DOLocalMove(endPos, (float)animationData.TotalDuration / 2));
        _currentSequence.Append(spriteTransform.DOLocalMove(startPos, (float)animationData.TotalDuration / 2));
        _sequences.Add(entity, _currentSequence);
    }
}