using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class DOTWEENAnimationDefinition : AnimationDefinition
{
    private Dictionary<Entity, Sequence> _sequences = new Dictionary<Entity, Sequence>();

    public override void InteruptAnimation(Entity entity)
    {
        if (_sequences.ContainsKey(entity))
        {
            _sequences[entity].Kill(true);
        }
    }

    protected override void OnTriggerAnimation(Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        Sequence sq = DOTween.Sequence();
        _sequences.SetOrAdd(entity, GetDOTWEENAnimationSequence(sq, entity, spriteStartPos, spriteTransform));
    }

    public virtual Sequence GetDOTWEENAnimationSequence(Sequence sq, Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        return sq;
    }
}