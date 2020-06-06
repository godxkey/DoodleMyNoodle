using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

public class GameActionShield : GameAction
{
    // TODO: add settings on the item itself
    const int HEAL = 100; // TEMP

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
        if (useData.TryGetParameter(0, out GameActionParameterSelfTarget.Popo paramTile))
        {
            CommonWrites.ModifyStatInt<Health>(accessor, instigatorPawn, HEAL);

#if UNITY_EDITOR
            DebugService.Log("hp SHIELD on " + accessor.GetName(instigatorPawnController));
#endif
        }
    }
}

