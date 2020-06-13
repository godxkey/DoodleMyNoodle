using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionEnrage : GameAction
{
    // TODO: add settings on the item itself
    const int HP_COST = 6;
    const int AP_GAIN = 2;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterSelfTarget.Description() { });
    }

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.HasComponent<Health>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterSelfTarget.Data self))
        {
            // reduce instigator HP
            CommonWrites.ModifyStatInt<Health>(accessor, context.InstigatorPawn, -HP_COST);

            // increase instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, AP_GAIN);
        }
    }
}
