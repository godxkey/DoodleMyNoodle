using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;
using CCC.Fix2D;

public class GameActionExplosion : GameAction<GameActionExplosion.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Damage;
        public fix Radius;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Damage = Damage,
                Radius = Radius
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Damage;
        public fix Radius;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        if (input.Accessor.TryGetComponent(input.ActionInstigatorActor, out FixTranslation translation))
        {
            CommonWrites.RequestExplosion(input.Accessor, input.ActionInstigator, translation.Value, settings.Radius, settings.Damage, false);
        }

        return true;
    }
}