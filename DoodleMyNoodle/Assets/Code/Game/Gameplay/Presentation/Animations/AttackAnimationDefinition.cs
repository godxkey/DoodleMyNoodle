using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Attack Animation")]
public class AttackAnimationDefinition : DOTWEENAnimationDefinition
{
    public override Sequence GetDOTWEENAnimationSequence(Sequence sq, Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        Vector2 startPos = spriteStartPos;
        Vector2 endPos = startPos + GetAnimationData<Vector2>("AttackVector");
        sq.Append(spriteTransform.DOLocalMove(endPos, Duration / 2));
        sq.Append(spriteTransform.DOLocalMove(startPos, Duration / 2));
        return sq;
    }
}