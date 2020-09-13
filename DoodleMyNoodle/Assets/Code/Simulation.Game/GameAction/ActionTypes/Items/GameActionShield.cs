using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class GameActionShield : GameAction
{
    // TODO: add settings on the item itself
    const int DURATION = 1;
    const int AP_COST = 2;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(new GameActionParameterSelfTarget.Description() { });
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
        return AP_COST;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterSelfTarget.Data self))
        {
            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST);

            if (accessor.TryGetComponentData(context.InstigatorPawn, out Invincible invincible))
            {
                // refresh duration if already invicible
                accessor.SetComponentData(context.InstigatorPawn, new Invincible() { Duration = max(DURATION, invincible.Duration) });
            }
            else
            {
                // add invicible
                accessor.AddComponentData(context.InstigatorPawn, new Invincible() { Duration = DURATION });
            }
        }
    }
}

