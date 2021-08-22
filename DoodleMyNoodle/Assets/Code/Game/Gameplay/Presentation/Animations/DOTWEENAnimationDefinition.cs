using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public abstract class DOTWEENAnimationDefinition : AnimationDefinition
{
    private Dictionary<Entity, Tween> _sequences = new Dictionary<Entity, Tween>();

    public override void InteruptAnimation(Entity entity)
    {
        if (_sequences.ContainsKey(entity))
        {
            _sequences[entity].Kill(true);
            _sequences.Remove(entity);
        }
    }

    protected override void OnTriggerAnimation(Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        _sequences.SetOrAdd(entity, GetDOTWEENAnimationSequence(entity, spriteStartPos, spriteTransform));
    }

    public abstract Tween GetDOTWEENAnimationSequence(Entity entity, Vector3 spriteStartPos, Transform spriteTransform);
}