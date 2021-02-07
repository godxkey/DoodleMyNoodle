using Bolt.Compiler.Utils;
using CCC.Fix2D.Authoring;
using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngineX;

public enum ViewTechType
{
    GameObject,
    Tile
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
[ExecuteAlways]
public class SimAsset : ConvertToEntityMultiWorld, IConvertGameObjectToEntity
{

    [FormerlySerializedAs("Guid")]
    [SerializeField] private string _guid;
    [SerializeField] private GameObject _bindedViewPrefab;
    [SerializeField] private TileBase _bindedViewTile;
    [SerializeField] ViewTechType _viewTechType = ViewTechType.GameObject;
    [SerializeField] bool _showGhost = true; // temporary
    [SerializeField] bool _hasTransform = true;

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
    public ViewTechType ViewTechType => _viewTechType;

    [NonSerialized] private SimAssetId _runtimeId;
    [NonSerialized] private bool _runtimeIdAssigned = false;

    public SimAssetId GetSimAssetId()
    {
        if (!_runtimeIdAssigned)
        {
            if (SimAssetBankInstance.Ready)
            {
                var lookup = SimAssetBankInstance.GetLookup();
                _runtimeId = lookup.GetRuntimeSimAssetId(Guid);
                _runtimeIdAssigned = true; 
            }
            else
            {
                Debug.LogError($"{nameof(SimAssetBankInstance)} is null. The converted SimAssetId will be invalid.");
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

        //if (_hasTransform && _bindedViewPrefab != null) // only add sim asset id if we have a binded view, otherwise its useless
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
            // rotation and scale are not yet handled by the game sim
        }
    }
#endif
}
