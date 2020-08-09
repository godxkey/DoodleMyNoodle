using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

[UpdateBefore(typeof(ExecutePawnControllerInputSystem))]
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
                    if (Accessor.HasComponent<Timer>(interactable))
                    {
                        shouldResetInteractable = false;
                        Timer currentInteractableTimer = Accessor.GetComponentData<Timer>(interactable);
                        if (currentInteractableTimer.Value <= 0)
                        {
                            Accessor.SetComponentData(interactable, new Timer() { Value = (fix)interactableData.Delay, CanCountdown = false });
                            shouldResetInteractable = true;
                        }
                    }

                    // can be interacted again, reset as not interacted
                    if (shouldResetInteractable) 
                        Accessor.SetComponentData(interactable, new Interacted() { Value = false });
                }
            }
        });
    }
}