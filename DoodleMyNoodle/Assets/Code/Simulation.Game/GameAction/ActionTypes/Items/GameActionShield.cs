using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

public class GameActionShield : GameAction
{
    // TODO: add settings on the item itself
    const int DURATION = 1;
    const int AP_COST = 2;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(new GameActionParameterSelfTarget.Description() {});
    }

    public override bool IsContextValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.HasComponent<Health>(context.InstigatorPawn)
            && accessor.HasComponent<ActionPoints>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterSelfTarget.Data self))
        {
            if (accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value < AP_COST)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST);

            accessor.AddComponentData(context.InstigatorPawn, new Invincible() { Duration = DURATION });
        }
    }
}

