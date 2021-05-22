using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;

public class GameActionConvert : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingRange),
        typeof(GameActionSettingEffectDuration)
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterEntity.Description()
            {
                RangeFromInstigator = accessor.GetComponent<GameActionSettingRange>(context.Item).Value,
                IncludeSelf = false,
                RequiresAttackableEntity = false,
                CustomPredicate = (simWorld, tileActor) =>
                {
                    if (simWorld.HasComponent<Controllable>(tileActor))
                    {
                        var pawnController = CommonReads.GetPawnController(simWorld, tileActor);

                        return simWorld.Exists(pawnController) && simWorld.HasComponent<Team>(pawnController);
                    }
                    return false;
                }
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterEntity.Data paramEntity))
        {
            var settingsRange = accessor.GetComponent<GameActionSettingRange>(context.Item);

            Entity target = paramEntity.Entity;

            if (!accessor.Exists(target))
            {
                LogGameActionInfo(context, "Target does not exit");
                return false;
            }

            if (!CommonReads.IsInRange(accessor, context.InstigatorPawn, target, settingsRange))
            {
                LogGameActionInfo(context, "Target not in range");
                return false;
            }

            if (!accessor.TryGetComponent(target, out Controllable controllable))
            {
                LogGameActionInfo(context, "Target not controllable");
                return false;
            }

            Entity targetController = controllable.CurrentController;
            if (!accessor.TryGetComponent(targetController, out Team currentTeam))
            {
                LogGameActionInfo(context, "Target has no team");
                return false;
            }

            var newTeam = currentTeam.Value == 0 ? 1 : 0;

            accessor.SetComponent(targetController, new Team() { Value = newTeam });
            if (accessor.HasComponent<Converted>(targetController))
            {
                accessor.RemoveComponent<Converted>(targetController);
            }
            else
            {
                accessor.AddComponent(targetController, new Converted() { RemainingTurns = accessor.GetComponent<GameActionSettingEffectDuration>(context.Item).Value });
            }

            return true;
        }

        return false;
    }
}
