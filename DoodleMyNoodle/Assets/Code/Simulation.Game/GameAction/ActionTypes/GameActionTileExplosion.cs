using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using CCC.Fix2D;

public class GameActionTileExplosion : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingDamage),
        typeof(GameActionSettingRadius),
        typeof(GameActionSettingRange),
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        var range = accessor.GetComponent<GameActionSettingRange>(context.Item);
        return new UseContract(
                   new GameActionParameterPosition.Description()
                   {
                       MaxRangeFromInstigator = range
                   });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            fix2 instigatorPos = accessor.GetComponent<FixTranslation>(context.InstigatorPawn);
            int damage = accessor.GetComponent<GameActionSettingDamage>(context.Item).Value;
            fix range = accessor.GetComponent<GameActionSettingRange>(context.Item).Value;
            fix radius = accessor.GetComponent<GameActionSettingRadius>(context.Item).Value;

            fix2 pos = Helpers.ClampPositionInsideRange(paramPosition.Position, instigatorPos, range);
            CommonWrites.RequestExplosion(accessor, context.InstigatorPawn, pos, radius, damage, true);

            return true;
        }

        return false;
    }
}