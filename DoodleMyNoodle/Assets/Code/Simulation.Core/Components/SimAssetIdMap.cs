using System;
using System.Collections.Generic;
using UnityEngine;

public class SimAssetIdMap : ScriptableObject
{
    public class LookUp
    {
        private readonly Dictionary<string, int> _map;

        public LookUp(SimAssetIdMap map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            _map = new Dictionary<string, int>(map._guids.Count);

            for (int i = 0; i < map._guids.Count; i++)
            {
                _map.Add(map._guids[i], i + 1);
            }
        }

        public SimAssetId EditIdToRuntimeId(string guid)
        {
            if (_map.TryGetValue(guid, out int runtimeValue))
            {
                return new SimAssetId(runtimeValue);
            }

            Debug.LogError($"[SimAssetIdMap.LookUp] Could not find runtime id for guid {guid}. " +
                $"Stop playing and try forcing an update with \"Tools > Data Management > Force Update SimAssetIds\"");
            return SimAssetId.Invalid;
        }
    }

    [SerializeField] private List<string> _guids = new List<string>();

#if UNITY_EDITOR
    public List<string> EditorGuids => _guids;
#endif
}