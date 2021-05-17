using static fixMath;
using Unity.Entities;
using Unity.Collections;
using CCC.Fix2D;
using System;

public class GameActionJump : GameAction
{
    static readonly fix MIN_VELOCITY = fix(0.05);

    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingThrow),
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        GameActionSettingThrow settings = accessor.GetComponentData<GameActionSettingThrow>(context.Item);

        return new UseContract(
            new GameActionParameterVector.Description()
            {
                SpeedMin = settings.SpeedMin,
                SpeedMax = settings.SpeedMax
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterVector.Data paramVector))
        {
            // get settings
            GameActionSettingThrow settings = accessor.GetComponentData<GameActionSettingThrow>(context.Item);

            fix2 velocity = paramVector.Vector;
            fix inputSpeed = length(velocity);
            fix2 dir = inputSpeed < MIN_VELOCITY ? fix2(0, 1) : velocity / inputSpeed;

            // Clamp vector and speed to min/max speed setting
            {
                if (inputSpeed < settings.SpeedMin)
                {
                    velocity = dir * settings.SpeedMin;
                }
                else if (inputSpeed > settings.SpeedMax)
                {
                    velocity = dir * settings.SpeedMax;
                }
            }

            CommonWrites.RequestImpulse(accessor, context.InstigatorPawn, velocity);

            return true;
        }

        return false;
    }
}