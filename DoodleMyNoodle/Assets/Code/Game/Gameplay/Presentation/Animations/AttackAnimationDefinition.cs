using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Attack Animation")]
public class AttackAnimationDefinition : AnimationDefinition
{
    private Dictionary<Entity, Sequence> _sequences = new Dictionary<Entity, Sequence>();

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
        _sequences.SetOrAdd(entity, _currentSequence);
    }
}