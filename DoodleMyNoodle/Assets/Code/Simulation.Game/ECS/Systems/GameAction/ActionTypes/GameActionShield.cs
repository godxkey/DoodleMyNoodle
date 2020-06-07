using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

public class GameActionShield : GameAction
{
    // TODO: add settings on the item itself
    const int DURATION = 1;
    const int AP_COST = 2;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        return new UseContract(new GameActionParameterSelfTarget.Description() {});
    }

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        return accessor.HasComponent<Health>(instigatorPawn)
            && accessor.HasComponent<ActionPoints>(instigatorPawn)
            && accessor.HasComponent<FixTranslation>(instigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn, UseData useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterSelfTarget.Data paramTile))
        {
            if (accessor.GetComponentData<ActionPoints>(instigatorPawn).Value < AP_COST)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, instigatorPawn, -AP_COST);

            accessor.AddComponentData(instigatorPawn, new Invincible() { Duration = DURATION });
        }
    }
}

