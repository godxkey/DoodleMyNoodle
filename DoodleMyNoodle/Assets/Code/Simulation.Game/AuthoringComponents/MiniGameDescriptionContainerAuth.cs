using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class MiniGameDescriptionContainerAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemActionPointCostData>
{
    public MiniGameDescriptionBase MiniGameDescription;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MiniGameDescriptionContainer() { MiniGame = MiniGameDescription });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemActionPointCostData inputData)
    {
        if (MiniGameDescription != null)
        {
            return "Has MiniGame";
        }

        return "Invalid MiniGame";
    }
}