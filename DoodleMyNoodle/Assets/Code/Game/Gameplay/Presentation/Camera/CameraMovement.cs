using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float CameraDisplacementFactor = 1;

    public Texture2D MouseIdle;
    public Texture2D MouseGrab;
    private CursorMode _cursorMode = CursorMode.Auto;
    private Vector2 _hotSpot = Vector2.zero;

    public float MaxPositionX;
    private float _minPositionX;

    private Camera _gameCamera;

    private Vector3 _previousMousePosition = new Vector3();
    private bool _isHolding = false;
    private bool _canMoveCamera = true;

    private void Start()
    {
        _gameCamera = GetComponent<Camera>();
        _minPositionX = _gameCamera.transform.position.x;
        _previousMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (!_canMoveCamera)
            return;

        if (Input.GetMouseButton(1))
        {
            if (!_isHolding)
            {
                _previousMousePosition = Input.mousePosition;

                _isHolding = true;

                Cursor.SetCursor(MouseGrab, _hotSpot, _cursorMode);
            }

            Vector3 newMousePosition = Input.mousePosition;

            float xDisplacement = (newMousePosition - _previousMousePosition).x * -1 * CameraDisplacementFactor;

            Vector3 currentCameraPosition = _gameCamera.transform.position;
            _gameCamera.transform.position = new Vector3(Mathf.Clamp(currentCameraPosition.x + xDisplacement, _minPositionX, MaxPositionX), currentCameraPosition.y, currentCameraPosition.z);

            _previousMousePosition = newMousePosition;
        }
        else
        {
            _isHolding = false;

            Cursor.SetCursor(MouseIdle, _hotSpot, _cursorMode);
        }
    }

    public void MouseChangedSection(bool isOnMenu)
    {
        _canMoveCamera = !isOnMenu;
    }
}
