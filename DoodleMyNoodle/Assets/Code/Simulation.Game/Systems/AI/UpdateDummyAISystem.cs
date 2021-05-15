using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct DummyAITag : IComponentData { }

public class UpdateDummyAISystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<DummyAITag>()
            .ForEach((ref ReadyForNextTurn readyForNextTurn) =>
            {
                readyForNextTurn.Value = true;
            }).Schedule();
    }
}