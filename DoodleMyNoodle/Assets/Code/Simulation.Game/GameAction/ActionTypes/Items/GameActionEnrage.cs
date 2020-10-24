using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionEnrage : GameAction
{
    const int AP_GAIN = 2;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameter.Description() { });
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        if (!accessor.HasComponent<Health>(context.InstigatorPawn))
        {
            debugReason?.Set("Pawn has no health");
            return false;
        }

        return true;
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return 0;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (parameters.TryGetParameter(0, out GameActionParameter.Data self))
        {
            return true;
        }

        return false;
    }
}
