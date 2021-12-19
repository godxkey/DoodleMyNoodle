using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using System.Collections.Generic;
using UnityEngine;
using CCC.Fix2D;
using System;
using Unity.MathematicsX;
using Unity.Entities;

public class GameActionMeleeAttack : GameAction<GameActionMeleeAttack.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range;
        public int Damage;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                Damage = Damage,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Range;
        public int Damage;
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

            fix damageMultipler = 1;
            if (useData.TryGetParameter(0, out GameActionParameterSuccessRate.Data successRate)) 
            {
                switch (successRate.SuccessRate)
                {
                    case SurveySuccessRating.One:
                        damageMultipler = 0;
                        break;
                    case SurveySuccessRating.Two:
                        damageMultipler = (fix)0.5f;
                        break;
                    case SurveySuccessRating.Three:
                        damageMultipler = 1;
                        break;
                    case SurveySuccessRating.Four:
                        damageMultipler = 2;
                        break;
                    case SurveySuccessRating.Five:
                        damageMultipler = 3;
                        break;
                    default:
                        break;
                }
            }

            CommonWrites.RequestDamage(accessor, context.InstigatorPawn, hits, fixMath.roundToInt(settings.Damage * damageMultipler));

            fix2 attackVector = attackPosition - instigatorPos;
            resultData.Add(new ResultDataElement() { AttackVector = attackVector });

            return true;
        }

        return false;
    }
}