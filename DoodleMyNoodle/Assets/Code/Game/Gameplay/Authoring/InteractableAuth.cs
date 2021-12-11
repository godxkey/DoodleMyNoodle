using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class InteractableAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool InteractableOnStart = true;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<InteractableFlag>(entity, InteractableOnStart);
    }
}