using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using CCC.Fix2D;

public class GameActionBasicJump : GameAction<GameActionBasicJump.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix JumpVelocity;
        public fix EnergyCost;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                JumpVelocity = JumpVelocity,
                EnergyCost = EnergyCost,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix JumpVelocity;
        public fix EnergyCost;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract();
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData, Settings settings)
    {
        accessor.SetComponent<NavAgentFootingState>(context.InstigatorPawn, NavAgentFooting.AirControl);

        var velocity = accessor.GetComponent<PhysicsVelocity>(context.InstigatorPawn);
        velocity.Linear.y = settings.JumpVelocity;
        accessor.SetComponent(context.InstigatorPawn, velocity);

        return true;
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason, Settings settings)
    {
        if (accessor.TryGetComponent(context.InstigatorPawn, out NavAgentFootingState footing))
        {
            if (footing.Value != NavAgentFooting.Ground && footing.Value != NavAgentFooting.Ladder)
            {
                debugReason?.Set("Pawn has no ground or ladder footing.");
                return false;
            }
        }

        return base.CanBeUsedInContextSpecific(accessor, context, debugReason, settings);
    }
}