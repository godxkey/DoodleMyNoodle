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
        Sequence sq = DOTween.Sequence();
        Vector2 startPos = spriteStartPos;
        Vector2 endPos = startPos + (Vector2)animationData.Direction;
        sq.Append(spriteTransform.DOLocalMove(endPos, (float)animationData.TotalDuration / 2));
        sq.Append(spriteTransform.DOLocalMove(startPos, (float)animationData.TotalDuration / 2));
        _sequences.SetOrAdd(entity, sq);
    }
}