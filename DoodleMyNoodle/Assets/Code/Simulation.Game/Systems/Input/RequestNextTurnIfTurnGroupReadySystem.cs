using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InputSystemGroup))]
public class RequestNextTurnIfTurnGroupReadySystem : SimGameSystemBase
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
        if (playingEntities.Length > 0)
        {
            bool everybodyReady = true;
            foreach (var controller in playingEntities)
            {
                if (!GetComponent<ReadyForNextTurn>(controller))
                {
                    everybodyReady = false;
                    break;
                }
            }

            if (everybodyReady)
            {
                CommonWrites.RequestNextTurn(Accessor);
            }
        }
    }
}
