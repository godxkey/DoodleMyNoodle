using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class SimAssetBank : ScriptableObject
{
    public class LookUp
    {
        private readonly Dictionary<string, SimAssetId> _editIdToRuntimeId;
        private readonly Dictionary<SimAssetId, SimAsset> _idToPrefab;
        private readonly SimAssetBank _bank;

        public LookUp(SimAssetBank bank)
        {
            if (bank == null)
                throw new ArgumentNullException(nameof(bank));

            _bank = bank;

            _editIdToRuntimeId = new Dictionary<string, SimAssetId>(bank._simAssets.Count);
            _idToPrefab = new Dictionary<SimAssetId, SimAsset>(bank._simAssets.Count);

            int count = bank._simAssets.Count;
            for (int i = 0; i < count; i++)
            {
                var asset = bank._simAssets[i];

                if (asset != null)
                {
                    var assetId = new SimAssetId(i + 1);

                    _editIdToRuntimeId.Add(asset.Guid, assetId);
                    _idToPrefab.Add(assetId, asset);
                }
            }
        }

        public ReadOnlyList<SimAsset> SimAssets => _bank._simAssets.AsReadOnlyNoAlloc();

        public SimAssetId EditIdToRuntimeId(string guid)
        {
            if (_editIdToRuntimeId.TryGetValue(guid, out SimAssetId runtimeValue))
            {
                return runtimeValue;
            }

            Debug.LogError($"[{nameof(SimAssetBank)}] Could not find runtime id for guid {guid}. " +
                $"Stop playing and try forcing an update with \"Tools > Data Management > Force Update SimAssetIds\"");
            return SimAssetId.Invalid;
        }

        public SimAsset IdToPrefab(SimAssetId simAssetId)
        {
            if (_idToPrefab.TryGetValue(simAssetId, out SimAsset runtimeValue))
            {
                return runtimeValue;
            }

            return null;
        }
    }

    public LookUp GetLookUp()
    {
        if (_lookUp == null)
        {
            _lookUp = new LookUp(this);
        }
        return _lookUp;
    }

    [SerializeField] private List<SimAsset> _simAssets = new List<SimAsset>();

    [NonSerialized] private LookUp _lookUp;

    private void OnDisable()
    {
        _lookUp = null;
    }

#if UNITY_EDITOR
    public List<SimAsset> EditorSimAssets => _simAssets;
#endif
}