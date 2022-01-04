using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using System.Collections.Generic;
using UnityEngine;
using CCC.Fix2D;
using System;
using Unity.MathematicsX;
using Unity.Entities;

public class GameActionMeleeImpulse : GameAction<GameActionMeleeImpulse.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Range;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        GameActionParameterPosition.Description tileParam = new GameActionParameterPosition.Description()
        {
            MaxRangeFromInstigator = settings.Range
        };

        GameActionParameterSuccessRate.Description successParam = new GameActionParameterSuccessRate.Description();

        return new UseContract(tileParam, successParam);
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, List<ResultDataElement> resultData, Settings settings)
    {
        if (useData.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            fix2 instigatorPos = accessor.GetComponent<FixTranslation>(context.InstigatorPawn);

            fix2 attackPosition = Helpers.ClampPositionInsideRange(paramPosition.Position, instigatorPos, settings.Range);
            fix attackRadius = (fix)0.1f;

            var hits = CommonReads.Physics.OverlapCircle(accessor, attackPosition, attackRadius, ignoreEntity: context.InstigatorPawn);

            fix ImpulseMultipler = 1;
            if (useData.TryGetParameter(1, out GameActionParameterSuccessRate.Data successRate, warnIfFailed: false))
            {
                switch (successRate.SuccessRate)
                {
                    case SurveySuccessRating.One:
                        ImpulseMultipler = 1;
                        break;
                    case SurveySuccessRating.Two:
                        ImpulseMultipler = (fix)1.5f;
                        break;
                    case SurveySuccessRating.Three:
                        ImpulseMultipler = 2;
                        break;
                    case SurveySuccessRating.Four:
                        ImpulseMultipler = 3;
                        break;
                    case SurveySuccessRating.Five:
                        ImpulseMultipler = 4;
                        break;
                    default:
                        break;
                }
            }

            fix2 attackVector = attackPosition - instigatorPos;
            attackVector = fixMath.rotate(attackVector, /* rotation towards up */);

            foreach (var hit in hits)
            {
                CommonWrites.RequestImpulse(accessor, hit.Entity, attackVector * ImpulseMultipler);
            }

            resultData.Add(new ResultDataElement() { AttackVector = attackVector });

            return true;
        }

        return false;
    }
}