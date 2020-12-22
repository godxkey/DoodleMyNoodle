using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Death Animation")]
public class DeathAnimationDefinition : AnimationDefinition
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
        Sequence currentSequence = DOTween.Sequence();
        currentSequence.Join(spriteTransform.DOLocalRotate(new Vector3(0, 0, -90), 1, RotateMode.LocalAxisAdd));
        currentSequence.Join(spriteTransform.DOLocalMoveY(-0.5f, 1));
        _sequences.SetOrAdd(entity, currentSequence);
    }
}