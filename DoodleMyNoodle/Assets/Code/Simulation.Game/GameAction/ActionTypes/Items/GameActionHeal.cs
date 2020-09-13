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
    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason  debugReason)
    {
        return true;
    }
    
    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.GetComponentData<ItemActionPointCostData>(context.Entity).Value;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description(accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
            {
                RequiresAttackableEntity = true,
            }
        };

        return useContract;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int healValue = accessor.GetComponentData<ItemHealthPointsToHealData>(context.Entity).Value;
            
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
