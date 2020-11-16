using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngineX;

public class SimAssetBank : ScriptableObject
{
    public class LookupData : IDisposable
    {
        public readonly Dictionary<string, SimAssetId> EditIdToRuntimeId;
        public readonly List<SimAsset> SimAssets;
        public readonly NativeList<ViewTechType> SimAssetTechTypes;

        public LookupData(SimAssetBank bank)
        {
            if (bank == null)
                throw new ArgumentNullException(nameof(bank));

            SimAssetTechTypes = new NativeList<ViewTechType>(Allocator.Persistent);
            SimAssets = new List<SimAsset>(bank._simAssets.Count);
            EditIdToRuntimeId = new Dictionary<string, SimAssetId>(bank._simAssets.Count);

            int count = bank._simAssets.Count;
            for (int i = 0; i < count; i++)
            {
                var asset = bank._simAssets[i];

                if (asset != null)
                {
                    int v = SimAssets.Count + 1;
                    if (v > ushort.MaxValue)
                    {
                        Log.Error($"Too many SimAssets! We are exceeding the maximal SimAssetId value of {ushort.MaxValue}.");
                        break;
                    }

                    SimAssetId assetId = new SimAssetId((ushort)v);

                    SimAssets.Add(asset);
                    EditIdToRuntimeId.Add(asset.Guid, assetId);
                    SimAssetTechTypes.Add(asset.ViewTechType);
                }
            }
        }

        public void Dispose()
        {
            SimAssetTechTypes.Dispose();
        }
    }

    public struct Lookup
    {
        private readonly LookupData _lookupData;

        public Lookup(LookupData lookupData)
        {
            _lookupData = lookupData;
        }

        public ReadOnlyList<SimAsset> SimAssets => _lookupData.SimAssets.AsReadOnlyNoAlloc();

        public SimAssetId GetRuntimeSimAssetId(string guid)
        {
            if (_lookupData.EditIdToRuntimeId.TryGetValue(guid, out SimAssetId runtimeValue))
            {
                return runtimeValue;
            }

            Debug.LogError($"[{nameof(SimAssetBank)}] Could not find runtime id for guid {guid}. " +
                $"Stop playing and try forcing an update with \"Tools > Data Management > Force Update SimAssetIds\"");
            return SimAssetId.Invalid;
        }

        public SimAsset GetSimAsset(SimAssetId simAssetId)
        {
            int i = simAssetId.Value - 1;
            if (i >= 0 && i < _lookupData.SimAssets.Count)
            {
                return _lookupData.SimAssets[i];
            }

            return null;
        }

        public bool TryGetViewTechType(SimAssetId simAssetId, out ViewTechType viewTechType)
        {
            int i = simAssetId.Value - 1;
            if (i >= 0 && i < _lookupData.SimAssetTechTypes.Length)
            {
                viewTechType = _lookupData.SimAssetTechTypes[i];
                return true;
            }

            viewTechType = default;
            return false;
        }
    }

    public struct JobLookup
    {
        private readonly NativeList<ViewTechType> _simAssetTechTypes;

        public JobLookup(LookupData lookupData)
        {
            _simAssetTechTypes = lookupData.SimAssetTechTypes;
        }

        public bool TryGetViewTechType(SimAssetId simAssetId, out ViewTechType viewTechType)
        {
            int i = simAssetId.Value - 1;
            if (i >= 0 && i < _simAssetTechTypes.Length)
            {
                viewTechType = _simAssetTechTypes[i];
                return true;
            }

            viewTechType = default;
            return false;
        }
    }

    [SerializeField] private List<SimAsset> _simAssets = new List<SimAsset>();

#if UNITY_EDITOR
    public List<SimAsset> EditorSimAssets => _simAssets;
#endif
}