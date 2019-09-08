using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Banque de blueprint utilisé pour nos premier test
/// </summary>
[CreateAssetMenu(fileName = "New SimBlueprint Bank", menuName = "DoodleMyNoodle/SimBlueprint Bank")]
public class SimBlueprintBank : ScriptableObject, ISimModuleBlueprintBank
{
    [System.Serializable]
    class PrefabAndBlueprintPair
    {
        public SimBlueprintId blueprintId;
        public SimEntity prefab;
    }

    [SerializeField]
    private List<PrefabAndBlueprintPair> prefabBlueprints;

    public SimBlueprint GetBlueprint(in SimBlueprintId blueprintId)
    {
        switch (blueprintId.Type)
        {
            default:
            case SimBlueprintId.BlueprintType.Invalid:
            {
                return null;
            }

            case SimBlueprintId.BlueprintType.Prefab:
            {
                for (int i = 0; i < prefabBlueprints.Count; i++)
                {
                    if (prefabBlueprints[i].blueprintId.Value == blueprintId.Value)
                        return new SimBlueprint(blueprintId, prefabBlueprints[i].prefab);
                }
                return null;
            }

            case SimBlueprintId.BlueprintType.SceneGameObject:
            {
                throw new NotImplementedException(); // TODO
            }
        }
    }

    // Part of the ISimBlueprintBank interface. This is going to get called when the simulation is disposed of.
    public void Dispose()
    {
    }
}
