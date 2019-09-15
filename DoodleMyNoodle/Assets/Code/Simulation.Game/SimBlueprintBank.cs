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
    public struct PrefabData
    {
        public SimBlueprintId BlueprintId;
        public SimEntity Prefab;

        [HideInInspector] // used for editor display purposes
        public string EntityName;
    }

    [SerializeField]
    private List<PrefabData> _prefabBlueprints = new List<PrefabData>();

    public SimBlueprint GetBlueprint(in SimBlueprintId blueprintId)
    {
        switch (blueprintId.Type)
        {
            default:
            case SimBlueprintId.BlueprintType.Invalid:
            {
                return default;
            }

            case SimBlueprintId.BlueprintType.Prefab:
            {
                for (int i = 0; i < _prefabBlueprints.Count; i++)
                {
                    if (_prefabBlueprints[i].BlueprintId.Value == blueprintId.Value)
                        return new SimBlueprint(blueprintId, _prefabBlueprints[i].Prefab);
                }
                return default;
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


#if UNITY_EDITOR
    public List<PrefabData> PrefabData_Editor
    {
        get => _prefabBlueprints;
        set => _prefabBlueprints = value;
    }
#endif
}
