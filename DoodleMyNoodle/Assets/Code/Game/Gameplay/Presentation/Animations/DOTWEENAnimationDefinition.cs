using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineX;

public abstract class DOTWEENAnimationDefinition : AnimationDefinition
{
    [FormerlySerializedAs("Duration")]
    [SerializeField] protected float _duration;

    private Dictionary<int, Tween> _sequences = new Dictionary<int, Tween>();

    public override void StopAnimation(StopInput input)
    {
        if (_sequences.TryGetValue(input.TriggerId, out Tween tween))
        {
            if (!ApplicationUtilityService.ApplicationIsQuitting)
                tween.Complete();
            tween.KillIfActive();
            _sequences.Remove(input.TriggerId);
        }
    }

    public override void TriggerAnimation(TriggerInput input, ref TriggerOuput output)
    {
        output.Duration = _duration;
        _sequences[input.TriggerId] = GetDOTWEENAnimationSequence(input);
    }

    public abstract Tween GetDOTWEENAnimationSequence(TriggerInput input);
}