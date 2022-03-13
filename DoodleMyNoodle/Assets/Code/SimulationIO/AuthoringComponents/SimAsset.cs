﻿using CCC.Fix2D.Authoring;
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public enum ViewTechType
{
    GameObject,
    Tile
}

[DisallowMultipleComponent]
[ExecuteAlways]
public class SimAsset : ConvertToEntityMultiWorld, IConvertGameObjectToEntity
{
    [FormerlySerializedAs("Guid")]
    [SerializeField] private string _guid;
    [SerializeField] private GameObject _bindedViewPrefab;
    [SerializeField] private TileBase _bindedViewTile;
    [SerializeField] ViewTechType _viewTechType = ViewTechType.GameObject;
    [SerializeField] bool _hasTransform = true;

#pragma warning disable CS0414 // Field assigned but never used
    [SerializeField] bool _showGhost = true; // temporary
#pragma warning restore CS0414 // Field assigned but never used

    public override GameWorldType WorldToConvertTo => GameWorldType.Simulation;

    protected override void Awake()
    {
        if (Application.isPlaying)
        {
            base.Awake(); // this fires the conversion
        }
    }

    public string Guid => _guid;
    public GameObject BindedViewPrefab => _bindedViewPrefab;
    public TileBase BindedViewTile => _bindedViewTile;
    public ViewTechType ViewTechType { get => _viewTechType; set => _viewTechType = value; }
    public bool HasTransform { get => _hasTransform; set => _hasTransform = value; }

    [NonSerialized] private SimAssetId _runtimeId;
    [NonSerialized] private int _runtimeIdAssignedAtResetCounter;


    private static int s_staticResetCounter = 0;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void StaticReset() => s_staticResetCounter++;

    public SimAssetId GetSimAssetId()
    {
        if (_runtimeIdAssignedAtResetCounter != s_staticResetCounter)
        {
            if (string.IsNullOrEmpty(Guid)) // not a prefab
            {
                _runtimeId = SimAssetId.Invalid;
                _runtimeIdAssignedAtResetCounter = s_staticResetCounter;
            }
            else
            {
                if (SimAssetBankInstance.Ready)
                {
                    var lookup = SimAssetBankInstance.GetLookup();
                    _runtimeId = lookup.GetRuntimeSimAssetId(Guid);

                    if (_runtimeId == SimAssetId.Invalid)
                    {
                        Debug.LogError($"[{nameof(SimAssetBank)}] Could not find runtime id for guid {Guid} and name {gameObject.name}. " +
                            $"Stop playing and try forcing an update with \"Tools > Data Management > Force Update SimAssetIds\"");
                    }

                    _runtimeIdAssignedAtResetCounter = s_staticResetCounter;
                }
                else
                {
                    Debug.LogError($"{nameof(SimAssetBankInstance)} is null. The converted SimAssetId will be invalid.");
                }
            }
        }

        return _runtimeId;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // remove native unity transform
        NoTransform.RemoveTransformComponents(entity, dstManager, conversionSystem);

        if (_hasTransform)
        {
            conversionSystem.World.GetExistingSystem<ConvertToFixTransformSystem>()?.ToConvert.Add(transform);
        }

        dstManager.AddComponentData(entity, GetSimAssetId());
    }

#if UNITY_EDITOR
    public void Editor_SetGuid(string value)
    {
        _guid = value;
    }

    public void Editor_SetBindedViewPrefab(GameObject value)
    {
        _bindedViewPrefab = value;
    }

    public void Editor_SetViewTechType(ViewTechType value)
    {
        _viewTechType = value;
    }

    public void Editor_SetShowGhost(bool showGhost)
    {
        _showGhost = showGhost;
    }

    [NonSerialized] private GameObject _viewGhost;
    [NonSerialized] private Transform _viewGhostTr;
    [NonSerialized] private bool _enabled;
    [NonSerialized] private Transform _tr;

    private void OnEnable()
    {
        _tr = transform;
        _enabled = true;
        UpdateViewGhost();
    }

    private void OnDisable()
    {
        _enabled = false;
        UpdateViewGhost();
    }

    private void Update()
    {
        _tr = transform;
        UpdateViewGhost();
    }

    private void UpdateViewGhost()
    {
        bool canHaveGhost = _enabled && !Application.isPlaying && BindedViewPrefab != null && _showGhost;

        if (canHaveGhost && _viewGhost == null)
        {
            _viewGhost = Instantiate(_bindedViewPrefab);
            _viewGhost.name = _bindedViewPrefab.name;
            _viewGhost.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector;

            _viewGhostTr = _viewGhost.transform;
            _viewGhost.AddComponent<ViewEditorGhost>().BindedSimGameObject = gameObject;
        }

        if (!canHaveGhost && _viewGhost != null)
        {
            DestroyImmediate(_viewGhost);
            _viewGhost = null;
        }

        if (_viewGhost != null)
        {
            _viewGhostTr.position = _tr.position;
            _viewGhostTr.rotation = _tr.rotation;
            // scale is not yet handled by the game sim
        }
    }
#endif
}
