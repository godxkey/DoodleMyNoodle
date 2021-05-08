using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;

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
        var range = accessor.GetComponentData<GameActionSettingRange>(context.Item);
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
            int damage = accessor.GetComponentData<GameActionSettingDamage>(context.Item).Value;

            fix radius = 1;
            if (accessor.HasComponent<GameActionSettingRadius>(context.Item))
            {
                radius = accessor.GetComponentData<GameActionSettingRadius>(context.Item).Value;
            }

            CommonWrites.RequestExplosion(accessor, context.InstigatorPawn, paramPosition.Position, radius, damage);

            return true;
        }

        return false;
    }
}