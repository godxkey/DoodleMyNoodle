using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;
using Unity.Collections;

public class GameActionHeal : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingRange),
        typeof(GameActionSettingHPToHeal)
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description(accessor.GetComponentData<GameActionSettingRange>(context.Item).Value)
            {
                RequiresAttackableEntity = true,
            }
        };

        return useContract;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int healValue = accessor.GetComponentData<GameActionSettingHPToHeal>(context.Item).Value;
            
            NativeList<Entity> targets = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindTileActorsWithComponents<Health>(accessor, paramTile.Tile, targets);
            foreach (var target in targets)
            {
                CommonWrites.RequestHeal(accessor, context.InstigatorPawn, target, healValue);
            }

            return true;
        }

        return false;
    }
}
