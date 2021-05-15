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
                RangeFromInstigator = accessor.GetComponentData<GameActionSettingRange>(context.Item).Value,
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
        if (parameters.TryGetParameter(0, out GameActionParameterEntity.Data paramTile))
        {
            // find target
            Entity target = CommonReads.Physics.FindFirstEntityWithComponentAtPosition<Controllable>(accessor, paramTile.EntityPos);
            Entity targetController = CommonReads.GetPawnController(accessor, target);
            if (accessor.TryGetComponentData(targetController, out Team currentTeam))
            {
                var newTeam = currentTeam.Value == 0 ? 1 : 0;

                accessor.SetComponentData(targetController, new Team() { Value = newTeam });
                if (accessor.HasComponent<Converted>(targetController))
                {
                    accessor.RemoveComponent<Converted>(targetController);
                }
                else
                {
                    accessor.AddComponentData(targetController, new Converted() { RemainingTurns = accessor.GetComponentData<GameActionSettingEffectDuration>(context.Item).Value });
                }

                return true;
            }
        }

        return false;
    }
}
