using System;
using UnityEngine;
using UnityEngineX;

public class CharacterOutsideScreenDisplay : GamePresentationBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _warningDisplay;

    protected override void OnGamePresentationUpdate()
    {
        if (_camera == null)
            return;

        Vector3 localPlayerPosition = SimWorldCache.LocalPawnPositionFloat;

        Vector3 viewportPlayerPosition = _camera.WorldToViewportPoint(localPlayerPosition);

        if ((viewportPlayerPosition.x < 0 || viewportPlayerPosition.x > 1) 
            || (viewportPlayerPosition.y < 0 || viewportPlayerPosition.y > 1))
        {
            _warningDisplay?.SetActive(true);
        }
        else
        {
            _warningDisplay?.SetActive(false);
        }
    }
}