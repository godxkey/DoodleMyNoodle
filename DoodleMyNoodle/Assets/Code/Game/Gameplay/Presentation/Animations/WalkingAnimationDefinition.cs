using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Move Animation")]
public class WalkingAnimationDefinition : DOTWEENAnimationDefinition
{
    public float WalkingHeight = 0.08f;

    public override Tween GetDOTWEENAnimationSequence(TriggerInput input)
    {
        Sequence sq = DOTween.Sequence();
        float walkingStartY = 0;
        float walkingEndY = walkingStartY + WalkingHeight;

        float animationSpeedOverride = -1;
        if (GamePresentationCache.Instance.SimWorld.TryGetBufferReadOnly(input.SimulationTarget, out DynamicBuffer<StatModifier> stats))
        {
            foreach (var stat in stats)
            {
                if (stat.Type == StatModifierType.Fast)
                {
                    animationSpeedOverride = 0.5f;
                    break;
                }
                else if (stat.Type == StatModifierType.Slow)
                {
                    animationSpeedOverride = 1.5f;
                    break;
                }
            }
        }

        sq.Join(input.PresentationTarget.Bone.DOLocalMoveY(walkingEndY, animationSpeedOverride == -1 ? _duration : animationSpeedOverride).SetEase(Ease.InOutQuad));

        sq.SetLoops(-1);
        return sq;
    }
}