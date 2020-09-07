using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

public class GameActionShield : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(new GameActionParameterSelfTarget.Description() {});
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        if (!accessor.HasComponent<Health>(context.InstigatorPawn))
        {
            debugReason?.Set("pawn has no health");
            return false;
        }

        return true;
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.GetComponentData<ItemActionPointCostData>(context.Entity).Value;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterSelfTarget.Data self))
        {
            accessor.AddComponentData(context.InstigatorPawn, new Invincible() { Duration = accessor.GetComponentData<ItemEffectDurationData>(context.Entity).Value });

            return true;
        }

        return false;
    }
}

