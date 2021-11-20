using CCC.Fix2D;
using Unity.Entities;

[UpdateInGroup(typeof(AISystemGroup))]
public class ClearAIDestinationWhenReadyForNextTurnSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref AIDestination destination, in ReadyForNextTurn readyForNextTurn) =>
        {
            if (readyForNextTurn)
            {
                destination.HasDestination = false;
            }
        }).Schedule();
    }
}
