using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;

public class GameActionJump : GameAction<GameActionJump.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix2 Direction;
        public fix Force;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Direction = Direction,
                Force = Force,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix2 Direction;
        public fix Force;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        CommonWrites.RequestImpulse(input.Accessor, input.Context.FirstPhysicalInstigator, settings.Direction * settings.Force, true);

        return true;
    }
}