using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class InteractableAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float InteractionRange = 1;
    public bool CanBeInteractedOnlyOnce = true;
    public float DelayBetweenInteraction = 0.1f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Interactable() { OnlyOnce = CanBeInteractedOnlyOnce, Range = InteractionRange ,Delay = DelayBetweenInteraction });
        if (!CanBeInteractedOnlyOnce)
        {
            dstManager.AddComponentData(entity, new Timer() { Value = (fix)DelayBetweenInteraction, CanCountdown = false });
        }
    }
}