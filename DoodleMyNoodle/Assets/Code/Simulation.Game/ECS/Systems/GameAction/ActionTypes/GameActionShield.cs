using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

public class GameActionShield : GameAction
{
    // TODO: add settings on the item itself
    const int DURATION = 1;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        return new UseContract(new GameActionParameterSelfTarget.Description() {});
    }

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        return accessor.HasComponent<Health>(instigatorPawn)
            && accessor.HasComponent<FixTranslation>(instigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn, UseData useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterSelfTarget.Data paramTile))
        {
            accessor.AddComponentData(instigatorPawnController, new Invincible() { Duration = DURATION });
        }
    }
}

