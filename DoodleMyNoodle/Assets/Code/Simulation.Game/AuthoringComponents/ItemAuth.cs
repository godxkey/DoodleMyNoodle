using CCC.InspectorDisplay;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct GameActionRequestReference
{
    public List<GameAction.ParameterDescriptionType> ParameterTypes;
    public GameActionRequestDefinitionBase Definition;
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public ItemVisualInfo ItemVisualInfo;

    public bool HasMiniGames = false;
    [ShowIf("HasMiniGames")]
    public List<GameActionRequestReference> RequestReference;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
    }

    public GameActionRequestDefinitionBase FindRequestDefinitionForParameters(params GameAction.ParameterDescription[] parameters)
    {
        foreach (GameActionRequestReference request in RequestReference)
        {
            bool hasAllTypes = true;
            foreach (GameAction.ParameterDescription parameter in parameters)
            {
                bool hasType = false;
                for (int i = 0; i < request.ParameterTypes.Count; i++)
                {
                    if (request.ParameterTypes[i] == parameter.GetParameterDescriptionType())
                    {
                        hasType = true;
                    }
                }

                if (!hasType)
                {
                    hasAllTypes = false;
                }
            }

            if (hasAllTypes)
            {
                return request.Definition;
            }
        }

        return null;
    }
}