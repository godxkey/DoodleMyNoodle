using Unity.Collections;
using Unity.Entities;
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
        typeof(GameActionDamageData),
        typeof(GameActionRangeData),
        typeof(GameActionAPCostData)
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        GameActionParameterTile.Description tileParam = new GameActionParameterTile.Description(accessor.GetComponentData<GameActionRangeData>(context.Item).Value)
        {
            IncludeSelf = false,
            RequiresAttackableEntity = true,
        };

        return new UseContract(tileParam);
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int2 instigatorTile = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn));

            // melee attack has a range of RANGE
            if (mathX.lengthmanhattan(paramTile.Tile - instigatorTile) > accessor.GetComponentData<GameActionRangeData>(context.Item).Value)
            {
                LogGameActionInfo(context, $"Melee attack at {paramTile.Tile} out of range. Ignoring.");
                return false;
            }

            // reduce target health
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindTileActorsWithComponents<Health>(accessor, paramTile.Tile, victims);

            int damageValue = accessor.GetComponentData<GameActionDamageData>(context.Item).Value;

            foreach (Entity entity in victims)
            {
                CommonWrites.RequestDamageOnTarget(accessor, context.InstigatorPawn, entity, damageValue);
            }

            int2 attackDirection = paramTile.Tile - instigatorTile;
            resultData.AddData(new KeyValuePair<string, object>("Direction", attackDirection));

            return true;
        }

        return false;
    }
}
