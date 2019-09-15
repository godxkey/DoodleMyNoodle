
using UnityEngine;

public static class SimBlueprintUtility
{
    public static SimBlueprintId GetSimBlueprintIdFromBakedSceneGameObject(GameObject gameobject)
    {
        return new SimBlueprintId(SimBlueprintId.BlueprintType.SceneGameObject, "TODO");
    }

    public static SimBlueprintId GetSimBlueprintIdFromBakedPrefabGUID(string guid)
    {
        return new SimBlueprintId(SimBlueprintId.BlueprintType.Prefab, guid);
    }


    // Used when reconstructing the simulation from a saved state
    public static GameObject GetBakedSceneGameObjectFromSimBlueprintId(SimBlueprintId blueprintId, GameObject[] sortedSceneRootGameObjects)
    {
        return null; // TODO
    }
}