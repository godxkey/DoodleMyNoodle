using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Attack Animation")]
public class AttackAnimationDefinition : DOTWEENAnimationDefinition
{
    public VFXDefinition VFXOnTarget = null;

    public override Tween GetDOTWEENAnimationSequence(Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        Sequence sq = DOTween.Sequence();

        Vector2 startPos = spriteStartPos;
        Vector2 endPos = startPos + GetAnimationData<Vector2>("AttackVector");

        if (VFXOnTarget != null)
        {
            Data.Add(new KeyValuePair<string, object>("Transform", spriteTransform));
            sq.Append(spriteTransform.DOLocalMove(endPos, Duration / 2).OnComplete(() => { VFXOnTarget.TriggerVFX(Data); }));
        }
        else
        {
            sq.Append(spriteTransform.DOLocalMove(endPos, Duration / 2));
        }

        sq.Append(spriteTransform.DOLocalMove(startPos, Duration / 2));

        return sq;
    }
}