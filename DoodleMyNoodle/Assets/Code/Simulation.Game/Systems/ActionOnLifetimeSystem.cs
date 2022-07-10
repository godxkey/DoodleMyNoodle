

using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;

public struct ActionOnLifetime : IBufferElementData
{
    public Entity Action;
    public fix Lifetime;
}

[UpdateBefore(typeof(ExecuteGameActionSystem))]
public partial class ExecuteActionOnLifetimeSystem : SimGameSystemBase
{
    private ExecuteGameActionSystem _gameActionSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _gameActionSystem = World.GetOrCreateSystem<ExecuteGameActionSystem>();
    }

    protected override void OnUpdate()
    {
        var gameActionRequets = _gameActionSystem.CreateRequestBuffer();

        Entities.ForEach((Entity instigator, DynamicBuffer<ActionOnLifetime> actionOnLifetimes, in Lifetime lifetime) =>
        {
            for (int i = actionOnLifetimes.Length - 1; i >= 0; i--)
            {
                var entry = actionOnLifetimes[i];
                if (entry.Lifetime < lifetime)
                {
                    gameActionRequets.Add(new GameActionRequest()
                    {
                        ActionEntity = entry.Action,
                        Instigator = instigator,
                        Target = Entity.Null
                    });
                }
            }

        }).Schedule();

        _gameActionSystem.HandlesToWaitFor.Add(Dependency);
    }
}