using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using System.Collections.Generic;

public class GameActionConvert : GameAction<GameActionConvert.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range;
        public int TurnDuration;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                TurnDuration = TurnDuration,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Range;
        public int TurnDuration;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract(
            new GameActionParameterEntity.Description()
            {
                RangeFromInstigator = settings.Range,
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

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterEntity.Data paramEntity))
        {
            Entity target = paramEntity.Entity;

            if (!accessor.Exists(target))
            {
                LogGameActionInfo(context, "Target does not exit");
                return false;
            }

            if (!CommonReads.IsInRange(accessor, context.InstigatorPawn, target, settings.Range))
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
                accessor.AddComponent(targetController, new Converted() { RemainingTurns = settings.TurnDuration });
            }

            return true;
        }

        return false;
    }
}
