using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using CCC.Fix2D;
using System.Collections.Generic;

public class GameActionTileExplosion : GameAction<GameActionTileExplosion.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Radius;
        public int Damage;
        public fix Range;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Radius = Radius,
                Damage = Damage,
                Range = Range,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Radius;
        public int Damage;
        public fix Range;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract(
                   new GameActionParameterPosition.Description()
                   {
                       MaxRangeFromInstigator = settings.Range
                   });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            fix2 instigatorPos = accessor.GetComponent<FixTranslation>(context.InstigatorPawn);
            int damage = settings.Damage;
            fix range = settings.Range;
            fix radius = settings.Radius;

            fix2 pos = Helpers.ClampPositionInsideRange(paramPosition.Position, instigatorPos, range);
            CommonWrites.RequestExplosion(accessor, context.InstigatorPawn, pos, radius, damage, true);

            return true;
        }

        return false;
    }
}