using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using CCC.Fix2D;

public class GameActionBasicJump : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[] { typeof(GameActionSettingBasicJump) };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract();
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        var settings = accessor.GetComponent<GameActionSettingBasicJump>(context.Item);

        accessor.SetComponent<NavAgentFootingState>(context.InstigatorPawn, NavAgentFooting.AirControl);

        var velocity = accessor.GetComponent<PhysicsVelocity>(context.InstigatorPawn);
        velocity.Linear.y = settings.JumpVelocity;
        accessor.SetComponent(context.InstigatorPawn, velocity);

        if (accessor.TryGetComponent(context.InstigatorPawn, out MoveEnergy moveEnergy))
        {
            CommonWrites.SetStatFix(accessor, context.InstigatorPawn, new MoveEnergy() { Value = moveEnergy.Value - settings.EnergyCost });
        }

        return true;
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        if (accessor.TryGetComponent(context.InstigatorPawn, out NavAgentFootingState footing))
        {
            if (footing.Value != NavAgentFooting.Ground && footing.Value != NavAgentFooting.Ladder)
            {
                debugReason?.Set("Pawn has no ground or ladder footing.");
                return false;
            }
        }

        if (accessor.TryGetComponent(context.InstigatorPawn, out MoveEnergy moveEnergy))
        {
            if (moveEnergy.Value <= 0)
            {
                debugReason?.Set("Pawn has not enough juice to jump");
                return false;
            }
        }

        return base.CanBeUsedInContextSpecific(accessor, context, debugReason);
    }
}