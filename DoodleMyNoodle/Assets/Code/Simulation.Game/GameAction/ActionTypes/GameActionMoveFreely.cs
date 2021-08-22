using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;

public class GameActionMoveFreely : GameAction
{
    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return 1;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract();
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData)
    {
        accessor.SetOrAddComponent(context.InstigatorPawn, new ActionPoints() { Value = accessor.GetComponent<MaximumFix<ActionPoints>>(context.InstigatorPawn).Value });

        return true;
    }
}