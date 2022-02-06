using Unity.Mathematics;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using CCC.Fix2D;
using System;
using Unity.MathematicsX;
using System.Collections.Generic;

public class GameActionComboAttack : GameAction<GameActionComboAttack.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public int Range;
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
        public int Range;
        public int Damage;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        var param = new GameActionParameterPosition.Description()
        {
            MaxRangeFromInstigator = settings.Range
        };

        return new UseContract(param, param);
    }

    public override bool Use(ISimGameWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, Settings settings)
    {
        var instigatorPos = accessor.GetComponent<FixTranslation>(context.InstigatorPawn);
        fix attackRadius = (fix)0.1f;
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data firstStrikePos))
        {
            var attackPos = Helpers.ClampPositionInsideRange(firstStrikePos.Position, instigatorPos, settings.Range);

            CommonReads.Physics.OverlapCircle(accessor, attackPos, attackRadius, hits, ignoreEntity: context.InstigatorPawn);

            resultData.Add(new ResultDataElement() { Position = attackPos });
        }

        if (parameters.TryGetParameter(1, out GameActionParameterPosition.Data secondStrikePos))
        {
            var attackPos = Helpers.ClampPositionInsideRange(secondStrikePos.Position, instigatorPos, settings.Range);

            CommonReads.Physics.OverlapCircle(accessor, attackPos, attackRadius, hits, ignoreEntity: context.InstigatorPawn);

            resultData.Add(new ResultDataElement() { Position = attackPos });
        }

        CommonWrites.RequestDamage(accessor, hits, settings.Damage);

        return true;
    }
}
