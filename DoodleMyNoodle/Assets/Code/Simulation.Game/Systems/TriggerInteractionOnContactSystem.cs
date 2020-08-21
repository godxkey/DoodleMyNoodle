using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class TriggerInteractionOnContactSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
        .WithAll<ControlledEntity>()
        .ForEach((ref FixTranslation translation) =>
        {
            int2 playerTile = Helpers.GetTile(translation);
            Entity interactableEntity = CommonReads.FindFirstTileActorWithComponent<Interactable>(Accessor, CommonReads.GetTileEntity(Accessor, playerTile));

            // Interactable on same tile as player
            if (interactableEntity != Entity.Null)
            {
                CommonWrites.Interact(Accessor, interactableEntity);
            }
        });
    }
}

internal partial class CommonWrites
{
    public static void Interact(ISimWorldReadWriteAccessor accessor, Entity interactableEntity)
    {
        if (accessor.HasComponent<Timer>(interactableEntity))
        {
            Timer interactableTimer = accessor.GetComponentData<Timer>(interactableEntity);
            fix endTime = accessor.Time.ElapsedTime + interactableTimer.Duration;
            accessor.SetComponentData(interactableEntity, new Timer() { Duration = interactableTimer.Duration, EndTime = endTime, CanCountdown = true });
        }

        accessor.SetOrAddComponentData(interactableEntity, new Interacted() { Value = true });
    }
}