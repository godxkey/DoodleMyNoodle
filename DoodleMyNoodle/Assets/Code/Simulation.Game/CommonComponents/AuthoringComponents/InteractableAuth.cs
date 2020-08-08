using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class InteractableAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool CanBeInteractedOnlyOnce = true;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Interactable() { OnlyOnce = CanBeInteractedOnlyOnce });
    }
}