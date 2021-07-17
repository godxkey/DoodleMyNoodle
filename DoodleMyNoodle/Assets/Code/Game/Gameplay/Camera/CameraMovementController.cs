using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.MathematicsX;
using UnityEditor;
using CCC.Fix2D;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovementController : GamePresentationSystem<CameraMovementController>
{
    [SerializeField] private Transform _wallpaper;
    [SerializeField] private float _wallpaperVerticalSize = 10f;

    [Header("Mouse Movement Edge Trigger")]
    public float ScreenEdgeBorderThickness = 5.0f;

    [Header("Camera Reference")]
    public Camera Cam;

    [Header("Movement Speeds")]
    public float Speed;
    public float ZoomSpeed;

    [Header("Draggin")]
    public float DragSpeed = 1;
    public float DragMaxSpeed = 1;

    [Header("Smooth Follow")]
    public float followSmoothSpeed = 0.125f;

    [Header("Limits")]
    public float MinZoom = 1;
    public float MaxZoom;
    public float BeginZoom = 2;

    private Transform _transform;
    private float2 _boundsMin;
    private float2 _boundsMax;
    private DirtyValue<Entity> _localPawn;
    private Vector3 dragOrigin;
    private Vector2 _lastMousePos;

    [ConsoleVar("CameraLimits", "Enable/Disable the game camera move limits.", Save = ConsoleVarAttribute.SaveMode.PlayerPrefs)]
    private static bool s_enableMovementLimits = true;

    [ConsoleVar("CameraMouseMovementEnabledWhenWindowed", "Enable/Disable the game camera moving when moving the mouse pointer near the edges of the screen.", Save = ConsoleVarAttribute.SaveMode.PlayerPrefs)]
    private static bool s_mouseMovementsEnabledWindowedMode = false;

    private static bool MouseMovementsEnabled
    {
        get
        {
            if (Application.isEditor || !Screen.fullScreen)
                return s_mouseMovementsEnabledWindowedMode;
            else
                return true;
        }
    }

    public Vector2 CamPosition
    {
        get => _transform.position;
        set
        {
            _transform.position = new Vector3(
                Mathf.Clamp(value.x, _boundsMin.x, _boundsMax.x),
                Mathf.Clamp(value.y, _boundsMin.y, _boundsMax.y),
                _transform.position.z);
        }
    }

    public float CamSize
    {
        get => Cam.orthographicSize;
        set
        {
            value = Mathf.Clamp(value, MinZoom, MaxZoom);
            if (_wallpaper != null)
                _wallpaper.localScale = Vector3.one * (value / (_wallpaperVerticalSize / 2f));
            Cam.orthographicSize = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _transform = transform;
        CamSize = BeginZoom;
    }

    public override void OnPostSimulationTick()
    {
        base.OnPostSimulationTick();
        CenterOnPawnIfTeleport();
    }

    protected override void OnGamePresentationUpdate() { }

    private void LateUpdate()
    {
        if (GameConsole.IsOpen())
        {
            return;
        }

        CenterOnPawnIfChange();

        UpdateMovement();

        UpdateBounds();
    }

    private void CenterOnPawnIfTeleport()
    {
        SimWorld.Entities.ForEach((ref TeleportEventData teleportEvent) =>
        {
            // center camera on pawn after teleport
            if (teleportEvent.Entity == Cache.LocalPawn)
            {
                CamPosition = Cache.LocalPawnPositionFloat;
            }
        });
    }

    private void CenterOnPawnIfChange()
    {
        // center camera on pawn if pawn change (or on game start)
        _localPawn.Set(Cache.LocalPawn);
        if (_localPawn.ClearDirty() && _localPawn.Get() != Entity.Null)
        {
            CamPosition = Cache.LocalPawnPositionFloat;
        }
    }

    private void UpdateMovement()
    {
        if (UIStateMachineController.Instance.CurrentSate.Type == UIStateType.Drawing
            || UIStateMachineController.Instance.CurrentSate.Type == UIStateType.BlockedGameplay)
            return;

        Vector2 movement = Vector2.zero;

        bool isMouseInsideScreen =
               Input.mousePosition.x <= Screen.width
            && Input.mousePosition.x >= 0
            && Input.mousePosition.y <= Screen.height
            && Input.mousePosition.y >= 0;

        bool useMouseMovements = isMouseInsideScreen && MouseMovementsEnabled;

        Vector2 currentMousePosition = Input.mousePosition;

        // Camera follow
        if (SimWorld.TryGetComponent(Cache.LocalPawn, out PhysicsVelocity velocity) && velocity.Linear.lengthSquared > 0)
        {
            CursorOverlayService.Instance.ResetCursorToDefault();
            CamPosition = Cache.LocalPawnPositionFloat;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                _lastMousePos = currentMousePosition;
                CursorOverlayService.Instance.SetCursorType(CursorOverlayService.CursorType.Grab);
            }

            // Camera Drag
            if (Input.GetMouseButton(0) && UIStateMachineController.Instance.CurrentSate.Type != UIStateType.ParameterSelection)
            {
                Vector2 lastMouseWorld = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(_lastMousePos);
                Vector2 currentMouseWorld = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(currentMousePosition);

                _lastMousePos = currentMousePosition;

                CamPosition += lastMouseWorld - currentMouseWorld;
            }
            else
            {
                // Camera edge move
                CursorOverlayService.Instance.ResetCursorToDefault();

                if (Input.GetKey(KeyCode.UpArrow) || (useMouseMovements && (Input.mousePosition.y >= (Screen.height - ScreenEdgeBorderThickness))))
                {
                    movement += Vector2.up;
                }

                if (Input.GetKey(KeyCode.DownArrow) || (useMouseMovements && (Input.mousePosition.y >= 0 && Input.mousePosition.y <= ScreenEdgeBorderThickness)))
                {
                    movement -= Vector2.up;
                }

                if (Input.GetKey(KeyCode.LeftArrow) || (useMouseMovements && (Input.mousePosition.x >= 0 && Input.mousePosition.x <= ScreenEdgeBorderThickness)))
                {
                    movement += Vector2.left;
                }

                if (Input.GetKey(KeyCode.RightArrow) || (useMouseMovements && (Input.mousePosition.x >= (Screen.width - ScreenEdgeBorderThickness))))
                {
                    movement += Vector2.right;
                }

                if (movement != Vector2.zero)
                {
                    movement.Normalize();
                    CamPosition += movement * Speed * CamSize * Time.deltaTime;
                }
            }

            if (Input.GetKey(KeyCode.LeftShift) && Cache.LocalPawn != Entity.Null)
            {
                fix3 playerPosition = SimWorld.GetComponent<FixTranslation>(Cache.LocalPawn).Value;
                Vector3 cameraPostion = transform.position;
                transform.position = new Vector3((float)playerPosition.x, (float)playerPosition.y, cameraPostion.z);
            }
        }

        // Zoom
        if (isMouseInsideScreen)
        {
            CamSize -= Input.mouseScrollDelta.y * ZoomSpeed;
        }
    }

    private void UpdateBounds()
    {
        float2 min = float2(float.MinValue, float.MinValue);
        float2 max = float2(float.MaxValue, float.MaxValue);

        if (s_enableMovementLimits == true)
        {
            if (SimWorld == null || !SimWorld.HasSingleton<GridInfo>())
                return;

            GridInfo gridRect = SimWorld.GetSingleton<GridInfo>();

            if (!gridRect.Size.Equals(int2(0, 0))) // if grid size is (0,0), there is no grid!
            {
                min = gridRect.TileMin - float2(0.5f, 0.5f);
                max = gridRect.TileMax + float2(0.5f, 0.5f);
            }
        }

        _boundsMin = min;
        _boundsMax = max;
        CamPosition = CamPosition;
    }

    public void TeleportCameraToPosition(Vector2 newPosition)
    {
        CamPosition = newPosition;
    }
}