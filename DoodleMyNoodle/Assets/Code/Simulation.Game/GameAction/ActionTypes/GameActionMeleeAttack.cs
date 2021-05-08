using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using System.Collections.Generic;
using UnityEngine;
using CCC.Fix2D;
using System;
using Unity.MathematicsX;

public class GameActionMeleeAttack : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingDamage),
        typeof(GameActionSettingRange),
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        var range = accessor.GetComponentData<GameActionSettingRange>(context.Item);
        GameActionParameterPosition.Description tileParam = new GameActionParameterPosition.Description()
        {
            MaxRangeFromInstigator = range.Value
        };

        return new UseContract(tileParam);
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            fix2 instigatorPos = accessor.GetComponentData<FixTranslation>(context.InstigatorPawn);

            // melee attack has a range of RANGE
            fix range = accessor.GetComponentData<GameActionSettingRange>(context.Item);
            int damage = accessor.GetComponentData<GameActionSettingDamage>(context.Item);

            fix2 attackVector = clampLength(paramPosition.Position - instigatorPos, 0, range);
            fix2 attackPosition = instigatorPos + attackVector;
            fix attackRadius = (fix)0.1f;

            foreach (DistanceHit hit in CommonReads.Physics.OverlapCircle(accessor, attackPosition, attackRadius, ignoreEntity: context.InstigatorPawn))
            {
                if (accessor.HasComponent<Health>(hit.Entity))
                {
                    CommonWrites.RequestDamage(accessor, context.InstigatorPawn, hit.Entity, damage);
                }
            }

            resultData.AddData(new KeyValuePair<string, object>("Direction", attackVector));

            return true;
        }

        return false;
    }
}