using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimulationExtensions
{
    public static SimEntity FindEntityDeepFromGameObjectGuid(this GameObject[] gameObjects, in SimBlueprintId simBlueprintId)
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if(TryFindEntityDeepFromGameObjectGuid(gameObjects[i], simBlueprintId, out SimEntity simEntity))
            {
                return simEntity;
            }
        }
        return null;
    }

    public static bool TryFindEntityDeepFromGameObjectGuid(GameObject gameObject, in SimBlueprintId simBlueprintId, out SimEntity result)
    {
        if (gameObject.GetComponent(out SimEntity simEntity))
        {
            // check gameobject
            if (SimBlueprintProviderSceneObject.CompareGameObjectGuid(simEntity.BlueprintId, simBlueprintId))
            {
                result = simEntity;
                return true;
            }

            // check children
            if (gameObject.HasComponent<SimTransformComponent>())
            {
                Transform tr = gameObject.transform;
                for (int i = 0; i < tr.childCount; i++)
                {
                    if (TryFindEntityDeepFromGameObjectGuid(tr.GetChild(i).gameObject, simBlueprintId, out result))
                    {
                        return true;
                    }
                }
            }
        }

        result = null;
        return false;
    }
}
