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
        var param = new GameActionParameterTile.Description(accessor.GetComponentData<GameActionSettingRange>(context.Item).Value)
        {
            IncludeSelf = false,
            RequiresAttackableEntity = true
        };

        return new UseContract(param, param);
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        int2 instigatorTile = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn));
        NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);

        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data firstTile))
        {
            // melee attack has a range
            if (mathX.lengthmanhattan(firstTile.Tile - instigatorTile) > accessor.GetComponentData<GameActionSettingRange>(context.Item).Value)
            {
                return false;
            }

            // find victims
            CommonReads.FindTileActorsWithComponents<Health>(accessor, firstTile.Tile, victims);
        }

        if (parameters.TryGetParameter(1, out GameActionParameterTile.Data secondTile))
        {
            // melee attack has a range of RANGE
            if (mathX.lengthmanhattan(secondTile.Tile - instigatorTile) > accessor.GetComponentData<GameActionSettingRange>(context.Item).Value)
            {
                return false;
            }

            // find victims
            CommonReads.FindTileActorsWithComponents<Health>(accessor, secondTile.Tile, victims);
        }

        foreach (Entity entity in victims)
        {
            AttackEntityOnTile(accessor, context, entity);
        }

        if (victims.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void AttackEntityOnTile(ISimWorldReadWriteAccessor accessor, UseContext context, Entity victim)
    {
        CommonWrites.RequestDamage(accessor, context.InstigatorPawn, victim, accessor.GetComponentData<GameActionSettingDamage>(context.Item).Value);
    }

}
