using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(SimAsset))]
public class LevelDefinitionAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public GlobalGameplaySettings GlobalGameplaySettings;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<LevelDefinitionTag>(entity);
    }
}
