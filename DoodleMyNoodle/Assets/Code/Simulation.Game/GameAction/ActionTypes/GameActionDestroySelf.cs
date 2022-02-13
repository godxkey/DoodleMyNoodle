using System.Collections.Generic;
using System;
using Unity.Entities;

public class GameActionDestroySelf : GameAction<GameActionDestroySelf.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
            });
        }
    }

    public struct Settings : IComponentData
    {
        public bool DeleteMeWhenYouAddOtherStuff; // this is needed to avoid 'Zero sized component' exception
    }

    public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Settings settings)
    {
        return new ExecutionContract();
    }

    public override bool Use(ISimGameWorldReadWriteAccessor accessor, in ExecutionContext context, UseParameters useData, List<ResultDataElement> resultData, Settings settings)
    {
        accessor.DestroyEntity(context.ActionInstigatorActor);

        return true;
    }
}