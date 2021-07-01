using static fixMath;
using Unity.Entities;
using Unity.Collections;
using CCC.Fix2D;
using System;

public class GameActionJump : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingThrow),
        typeof(GameActionSettingIgnoreSurvey),
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        GameActionSettingThrow settings = accessor.GetComponent<GameActionSettingThrow>(context.Item);

        return new UseContract(
            new GameActionParameterVector.Description()
            {
                SpeedMin = settings.SpeedMin,
                SpeedMax = settings.SpeedMax
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        GameActionSettingThrow settings = accessor.GetComponent<GameActionSettingThrow>(context.Item);

        if (parameters.TryGetParameter(0, out GameActionParameterVector.Data paramVector))
        {
            fix2 velocity = paramVector.Vector;

            velocity = clampLength(velocity, settings.SpeedMin, settings.SpeedMax);

            CommonWrites.RequestImpulse(accessor, context.InstigatorPawn, velocity, ignoreMass: true);

            return true;
        }
        else
        {
            CommonWrites.RequestImpulse(accessor, context.InstigatorPawn, new fix2(0, settings.SpeedMin), ignoreMass: true);

            return true;
        }
    }
}