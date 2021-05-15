using Unity.Mathematics;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using CCC.Fix2D;
using System;
using Unity.MathematicsX;

public class GameActionComboAttack : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingRange),
        typeof(GameActionSettingDamage),
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        var param = new GameActionParameterPosition.Description()
        {
            MaxRangeFromInstigator = accessor.GetComponentData<GameActionSettingRange>(context.Item)
        };

        return new UseContract(param, param);
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        int damage = accessor.GetComponentData<GameActionSettingDamage>(context.Item);
        var instigatorPos = accessor.GetComponentData<FixTranslation>(context.InstigatorPawn);
        var range = accessor.GetComponentData<GameActionSettingRange>(context.Item);
        fix attackRadius = (fix)0.1f;
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data firstStrikePos))
        {
            var attackPos = Helpers.ClampPositionInsideRange(firstStrikePos.Position, instigatorPos, range);

            CommonReads.Physics.OverlapCircle(accessor, attackPos, attackRadius, hits, ignoreEntity: context.InstigatorPawn);
        }

        if (parameters.TryGetParameter(1, out GameActionParameterPosition.Data secondStrikePos))
        {
            var attackPos = Helpers.ClampPositionInsideRange(secondStrikePos.Position, instigatorPos, range);

            CommonReads.Physics.OverlapCircle(accessor, attackPos, attackRadius, hits, ignoreEntity: context.InstigatorPawn);
        }

        CommonWrites.RequestDamage(accessor, context.InstigatorPawn, hits, damage);

        return true;
    }
}
