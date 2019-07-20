using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Banque de blueprint utilisé pour nos premier test
/// </summary>
[CreateAssetMenu(fileName ="New SimBlueprint Bank", menuName = "DoodleMyNoodle/SimBlueprint Bank")]
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
        switch (blueprintId.type)
        {
            default:
            case SimBlueprintId.Type.Invalid:
            {
                return null;
            }

            case SimBlueprintId.Type.Prefab:
            {
                for (int i = 0; i < prefabBlueprints.Count; i++)
                {
                    if (prefabBlueprints[i].blueprintId.value == blueprintId.value)
                        return new SimBlueprint(blueprintId, prefabBlueprints[i].prefab);
                }
                return null;
            }

            case SimBlueprintId.Type.SceneGameObject:
            {
                throw new NotImplementedException(); // TODO
            }
        }
    }
}
