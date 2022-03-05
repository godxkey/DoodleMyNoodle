using CCC.Fix2D;
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

    public override Tween GetDOTWEENAnimationSequence(TriggerInput input)
    {
        Sequence sq = DOTween.Sequence();

        GameAction.ResultDataElement resultData = input.GetGameActionResultData();

        Vector2 startPos = Vector2.zero;
        Vector2 endPos = startPos;
        if (resultData.AttackVector.ToUnityVec() != Vector2.zero)
        {
            endPos += resultData.AttackVector.ToUnityVec();
        }
        else if (resultData.Position.ToUnityVec() != Vector2.zero)
        {
            endPos = resultData.Position.ToUnityVec() - (Vector2)input.PresentationTarget.Bone.position;
        }
        else if ((resultData.Entity != Entity.Null) && PresentationHelpers.GetSimulationWorld().TryGetComponent(resultData.Entity, out FixTranslation translation))
        {
            endPos = translation.Value.ToUnityVec() - (Vector2)input.PresentationTarget.Bone.position;
        }

        if (VFXOnTarget != null)
        {
            var vfxParams = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("Transform", input.PresentationTarget.Bone),
                new KeyValuePair<string, object>("AttackVector", resultData.AttackVector),
                new KeyValuePair<string, object>("InstigatorStartPosition", input.PresentationTarget.Bone.position),
            };
            sq.Append(input.PresentationTarget.Bone.DOLocalMove(endPos, _duration / 2)
                .OnComplete(() => VFXOnTarget.TriggerVFX(vfxParams)));
        }
        else
        {
            sq.Append(input.PresentationTarget.Bone.DOLocalMove(endPos, _duration / 2));
        }

        sq.Append(input.PresentationTarget.Bone.DOLocalMove(startPos, _duration / 2));

        return sq;
    }
}