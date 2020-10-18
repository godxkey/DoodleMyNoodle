using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineX;

[DisallowMultipleComponent]
[RequiresEntityConversion]
[ExecuteAlways]
public class SimAsset : ConvertToEntityMultiWorld, IConvertGameObjectToEntity
{
    public enum TechType
    {
        GameObject,
        Entity
    }

    [FormerlySerializedAs("Guid")]
    [SerializeField] private string _guid;
    [SerializeField] private GameObject _bindedViewPrefab;
    [SerializeField] TechType _viewTechType = TechType.GameObject;
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
    public TechType ViewTechType => _viewTechType;

    public SimAssetId GetSimAssetId()
    {
        if (SimAssetIdMapInstance.Get() != null)
        {
            return SimAssetIdMapInstance.Get().EditIdToRuntimeId(Guid);
        }
        else
        {
            Debug.LogError("SimAssetIdMapInstance is null. The converted SimAssetId will be invalid.");
        }

        return SimAssetId.Invalid;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // remove native unity transform
        NoTransform.RemoveTransformComponents(entity, dstManager, conversionSystem);

        if (_hasTransform)
        {
            var tr = transform;
            FixTransformAuth.AddFixTransformComponents(entity, dstManager, conversionSystem, tr.localPosition, tr.localRotation, tr.localScale);
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

    public void Editor_SetViewTechType(TechType value)
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
            SetHideFlagsOnChildren(_viewGhost, HideFlags.DontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector);

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

    private void SetHideFlagsOnChildren(GameObject go, HideFlags hideFlags)
    {
        go.hideFlags = hideFlags;
        foreach (Transform child in go.transform)
        {
            SetHideFlagsOnChildren(child.gameObject, hideFlags);
        }
    }
#endif
}
