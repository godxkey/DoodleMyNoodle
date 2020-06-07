using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionEnrage : GameAction
{
    // TODO: add settings on the item itself
    const int HP_COST = 2;
    const int AP_GAIN = 2;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        return new UseContract(
            new GameActionParameterSelfTarget.Description() { });
    }

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        return accessor.HasComponent<Health>(instigatorPawn)
            && accessor.HasComponent<FixTranslation>(instigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn, UseData useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterSelfTarget.Data self))
        {
            // reduce instigator HP
            CommonWrites.ModifyStatInt<Health>(accessor, instigatorPawn, -HP_COST);

            // increase instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, instigatorPawn, AP_GAIN);
        }
    }
}
