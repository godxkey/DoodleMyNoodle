using System;
using UnityEngine;
using UnityEngineX;

public class PingManager : GamePresentationSystem<PingManager>
{
    [SerializeField] private GameObject _pingPrefab;

    private GameObject _currentPing;

    protected override void OnGamePresentationUpdate()
    {
        if (_pingPrefab != null && Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            currentMousePosition.z = 10;
            _currentPing = Instantiate(_pingPrefab, CameraService.Instance.ActiveCamera.ScreenToWorldPoint(currentMousePosition), Quaternion.identity);
        }
    }
}