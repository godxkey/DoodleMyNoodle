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
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description(accessor.GetComponentData<GameActionRangeData>(context.Entity).Value)
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
            int healValue = accessor.GetComponentData<GameActionHPToHealData>(context.Entity).Value;
            
            NativeList<Entity> targets = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindTileActorsWithComponents<Health>(accessor, paramTile.Tile, targets);
            foreach (var target in targets)
            {
                CommonWrites.RequestHealOnTarget(accessor, context.InstigatorPawn, target, healValue);
            }

            return true;
        }

        return false;
    }
}
