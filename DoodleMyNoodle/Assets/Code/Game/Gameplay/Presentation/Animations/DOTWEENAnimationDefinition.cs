using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public abstract class DOTWEENAnimationDefinition : AnimationDefinition
{
    private Dictionary<Entity, Tween> _sequences = new Dictionary<Entity, Tween>();

    public override void FinishAnimation(Entity entity)
    {
        if (_sequences.TryGetValue(entity, out Tween tween))
        {
            tween.Kill(complete: true);
            _sequences.Remove(entity);
        }
    }

    public override void StopAnimation(Entity entity)
    {
        if (_sequences.TryGetValue(entity, out Tween tween))
        {
            tween.KillIfActive();
            _sequences.Remove(entity);
        }
    }

    protected override void OnTriggerAnimation(Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        _sequences[entity] = GetDOTWEENAnimationSequence(entity, spriteStartPos, spriteTransform);
    }

    public abstract Tween GetDOTWEENAnimationSequence(Entity entity, Vector3 spriteStartPos, Transform spriteTransform);
}