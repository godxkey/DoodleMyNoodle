using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Death Animation")]
public class DeathAnimationDefinition : DOTWEENAnimationDefinition
{
    public override Tween GetDOTWEENAnimationSequence(TriggerInput input)
    {
        Sequence sq = DOTween.Sequence();
        sq.Join(input.PresentationTarget.Bone.DOLocalRotate(new Vector3(0, 0, -90), _duration, RotateMode.LocalAxisAdd));
        sq.Join(input.PresentationTarget.Bone.DOLocalMoveY(-0.5f, _duration));
        return sq;
    }
}