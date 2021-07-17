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

        return new UseContract(tileParam);
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData, Settings settings)
    {
        if (useData.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            fix2 instigatorPos = accessor.GetComponent<FixTranslation>(context.InstigatorPawn);

            fix2 attackPosition = Helpers.ClampPositionInsideRange(paramPosition.Position, instigatorPos, settings.Range);
            fix attackRadius = (fix)0.1f;

            var hits = CommonReads.Physics.OverlapCircle(accessor, attackPosition, attackRadius, ignoreEntity: context.InstigatorPawn);
            CommonWrites.RequestDamage(accessor, context.InstigatorPawn, hits, settings.Damage);

            fix2 attackVector = attackPosition - instigatorPos;
            resultData.AddData(new KeyValuePair<string, object>("Direction", attackVector));

            return true;
        }

        return false;
    }
}