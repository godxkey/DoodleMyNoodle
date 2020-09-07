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

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        return true;
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return 0;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        CommonWrites.Interact(accessor, context.Entity, context.InstigatorPawn);

        return true;
    }
}