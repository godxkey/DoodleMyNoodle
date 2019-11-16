using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Banque de blueprint utilisé pour nos premier test
/// </summary>
[CreateAssetMenu(fileName = "New SimPrefab Bank", menuName = "DoodleMyNoodle/Advanced/SimPrefab Bank")]
public class SimPrefabBank : ScriptableObject
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
    internal List<PrefabData> _PrefabBlueprints = new List<PrefabData>();
    public ReadOnlyList<PrefabData> PrefabBlueprints => new ReadOnlyList<PrefabData>(_PrefabBlueprints);
}
