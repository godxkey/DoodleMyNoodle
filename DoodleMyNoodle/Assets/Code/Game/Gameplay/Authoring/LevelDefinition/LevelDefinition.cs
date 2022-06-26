using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(LevelDefinition))]
public class LevelDefinition : MonoBehaviour, IConvertGameObjectToEntity
{
    public GlobalGameplaySettings GlobalGameplaySettings;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<LevelDefinitionTag>(entity);
    }
}
