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

    public override Tween GetDOTWEENAnimationSequence(Entity entity, Vector3 spriteStartPos, Transform spriteTransform)
    {
        Sequence sq = DOTween.Sequence();

        GameAction.ResultDataElement resultData = GetGameActionResultData();

        Vector2 startPos = spriteStartPos;
        Vector2 endPos = startPos;
        if (resultData.AttackVector.ToUnityVec() != Vector2.zero)
        {
            endPos += resultData.AttackVector.ToUnityVec();
        }
        else if (resultData.Position.ToUnityVec() != Vector2.zero)
        {
            endPos = resultData.Position.ToUnityVec() - (Vector2)spriteTransform.position;
        }
        else if ((resultData.Entity != Entity.Null) && PresentationHelpers.GetSimulationWorld().TryGetComponent(resultData.Entity, out FixTranslation translation))
        {
            endPos = translation.Value.ToUnityVec() - (Vector2)spriteTransform.position;
        }

        if (VFXOnTarget != null)
        {
            Data.Add(new KeyValuePair<string, object>("Transform", spriteTransform));
            Data.Add(new KeyValuePair<string, object>("AttackVector", resultData.AttackVector));
            Data.Add(new KeyValuePair<string, object>("InstigatorStartPosition", spriteTransform.position));
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