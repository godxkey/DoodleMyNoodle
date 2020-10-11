using System;
using UnityEngine;
using UnityEngineX;

public class CharacterOutsideScreenDisplay : GamePresentationBehaviour
{
    [SerializeField] private GameObject _warningDisplay;

    protected override void OnGamePresentationUpdate()
    {
        Vector3 localPlayerPosition = Cache.LocalPawnPositionFloat;

        Vector3 viewportPlayerPosition = CameraService.Instance.ActiveCamera.WorldToViewportPoint(localPlayerPosition);

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