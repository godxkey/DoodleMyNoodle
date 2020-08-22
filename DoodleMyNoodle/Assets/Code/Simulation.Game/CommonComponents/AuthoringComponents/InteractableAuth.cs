using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class InteractableAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool InteractOnContact = false;
    public float InteractionRange = 1;
    public bool CanBeInteractedOnlyOnce = true;
    public float DelayBetweenInteraction = 0.1f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Interactable() { OnlyOnce = CanBeInteractedOnlyOnce, Range = (fix)InteractionRange , Delay = (fix)DelayBetweenInteraction });
        if (!CanBeInteractedOnlyOnce)
        {
            dstManager.AddComponentData(entity, new NoInteractTimer() { Duration = (fix)DelayBetweenInteraction, CanCountdown = false });
        }

        if (InteractOnContact)
        {
            dstManager.AddComponentData(entity, new InteractOnOverlapTag());
        }
    }
}