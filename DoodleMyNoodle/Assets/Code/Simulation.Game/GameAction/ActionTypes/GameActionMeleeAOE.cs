using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using CCC.Fix2D;
using System.Collections.Generic;

public class GameActionMeleeAOE : GameAction<GameActionMeleeAOE.Settings>
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

        return new UseContract(tileParam);
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, List<ResultDataElement> resultData, Settings settings)
    {
        if (useData.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            fix2 instigatorPos = accessor.GetComponent<FixTranslation>(context.InstigatorPawn);
            fix2 attackPosition = Helpers.ClampPositionInsideRange(paramPosition.Position, instigatorPos, settings.Range);

            var hits = CommonReads.Physics.OverlapAabb(accessor, Helpers.GetTileBottom(instigatorPos), Helpers.GetTileCenter(attackPosition), context.InstigatorPawn);

            CommonWrites.RequestDamage(accessor, context.InstigatorPawn, hits, fixMath.ceilToInt(settings.Damage));

            fix2 attackVector = attackPosition - instigatorPos;
            resultData.Add(new ResultDataElement() { AttackVector = attackVector });

            return true;
        }

        return false;
    }
}