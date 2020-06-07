using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

public class LocalPawnHighlightController : GamePresentationBehaviour
{
    [SerializeField] private GameObject _highlightPrefab;

    private Transform _highlight;

    public override void OnGameAwake()
    {
        _highlight = Instantiate(_highlightPrefab, transform).transform;
    }

    public override void OnGameLateUpdate()
    {
        if(SimWorldCache.LocalPawn != Entity.Null)
        {
            _highlight.gameObject.SetActive(true);
            _highlight.transform.position = SimWorldCache.LocalPawnPositionFloat;
        }
        else
        {
            _highlight.gameObject.SetActive(false);
        }
    }
}
