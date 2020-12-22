using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Move Animation")]
public class MoveAnimationDefinition : AnimationDefinition
{
    public float WalkingHeight = 0.08f;

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
        Sequence currentSequence = DOTween.Sequence();
        float walkingStartY = spriteStartPos.y;
        float walkingEndY = walkingStartY + WalkingHeight;
        currentSequence.Append(spriteTransform.DOLocalMoveY(walkingEndY, (float)animationData.TotalDuration).SetEase(Ease.InOutQuad));
        currentSequence.SetLoops(-1);
        _sequences.SetOrAdd(entity, currentSequence);
    }
}