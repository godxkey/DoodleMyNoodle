using CCC.InspectorDisplay;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public ItemVisualInfo ItemVisualInfo;

    public bool HasMiniGame = false;
    [ShowIf("HasMiniGame")]
    public MiniGameDefinitionBase MiniGame;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
    }
}