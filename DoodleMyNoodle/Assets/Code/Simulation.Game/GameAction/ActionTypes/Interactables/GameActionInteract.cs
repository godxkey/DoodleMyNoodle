using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngineX;

public class GameActionInteract : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract();
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return 0;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        CommonWrites.Interact(accessor, context.Entity, context.InstigatorPawn);

        return true;
    }
}