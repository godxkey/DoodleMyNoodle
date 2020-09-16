using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

[UpdateBefore(typeof(InputSystemGroup))]
public class ResetInteractablesSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        ResetAllUsedOnceInteractable();
    }

    private void ResetAllUsedOnceInteractable()
    {
        Accessor.Entities.ForEach((Entity interactable, ref Interactable interactableData, ref Interacted interactedData) =>
        {
            if (interactedData.Value)
            {
                if (interactableData.OnlyOnce)
                {
                    // can never be interacted again
                    PostUpdateCommands.RemoveComponent<Interactable>(interactable);
                }
                else
                {
                    bool shouldResetInteractable = true;
                    if (Accessor.HasComponent<NoInteractTimer>(interactable))
                    {
                        shouldResetInteractable = false;
                        NoInteractTimer currentInteractableTimer = Accessor.GetComponentData<NoInteractTimer>(interactable);
                        if (Accessor.Time.ElapsedTime >= currentInteractableTimer.EndTime)
                        {
                            Accessor.SetComponentData(interactable, new NoInteractTimer() { Duration = interactableData.Delay, CanCountdown = false });
                            shouldResetInteractable = true;
                        }
                    }

                    // can be interacted again, reset as not interacted
                    if (shouldResetInteractable) 
                        Accessor.SetComponentData(interactable, new Interacted() { Value = false, Instigator = interactedData.Instigator });
                }
            }
        });
    }
}