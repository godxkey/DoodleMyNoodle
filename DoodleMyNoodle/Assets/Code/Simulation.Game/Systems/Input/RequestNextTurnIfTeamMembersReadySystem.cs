using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InputSystemGroup))]
public class RequestNextTurnIfTeamMembersReadySystem : SimSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<TurnSystemDataTag>();
    }

    protected override void OnUpdate()
    {
        NativeList<Entity> playingEntities = new NativeList<Entity>(Allocator.Temp);
        CommonReads.GetCurrentlyPlayingEntities(Accessor, playingEntities);

        // if any playing controller is active and ready, request next turn
        bool isSomeoneReady = false;
        foreach (var controller in playingEntities)
        {
            if (GetComponent<Active>(controller) && GetComponent<ReadyForNextTurn>(controller))
            {
                isSomeoneReady = true;
                break;
            }
        }

        if (isSomeoneReady)
        {
            CommonWrites.RequestNextTurn(Accessor);
        }
    }
}
