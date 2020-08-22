using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovementController : GamePresentationBehaviour
{
    [Header("Mouse Movement Edge Trigger")]
    public float ScreenEdgeBorderThickness = 5.0f;

    [Header("Camera Reference")]
    public Camera Cam;

    [Header("Movement Speeds")]
    public float Speed;
    public float ZoomSpeed;

    [Header("Limits")]
    public bool EnableMovementLimits;
    public float MaxZoom;

    private Transform _transform;
    private float2 _boundsMin;
    private float2 _boundsMax;

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

    private Vector2 CamPosition
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

    private float CamSize
    {
        get => Cam.orthographicSize;
        set => Cam.orthographicSize = Mathf.Clamp(value, 1, MaxZoom);
    }

    protected override void Awake()
    {
        base.Awake();

        _transform = transform;
    }

    public override void OnPostSimulationTick()
    {
        base.OnPostSimulationTick();

        SimWorld.Entities.ForEach((ref TeleportEventData teleportEvent) =>
        {
            // center camera on pawn after teleport
            if (teleportEvent.Entity == SimWorldCache.LocalPawn)
            {
                CamPosition = SimWorldCache.LocalPawnPositionFloat;
            }
        });
    }

    protected override void OnGamePresentationUpdate() { }

    void Update()
    {
        if (GameConsole.IsOpen())
        {
            return;
        }

        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || (MouseMovementsEnabled && (Input.mousePosition.y >= (Screen.height - ScreenEdgeBorderThickness))))
        {
            movement += Vector2.up;
        }

        if (Input.GetKey(KeyCode.S) || (MouseMovementsEnabled && (Input.mousePosition.y > 0 && Input.mousePosition.y <= ScreenEdgeBorderThickness)))
        {
            movement -= Vector2.up;
        }

        if (Input.GetKey(KeyCode.A) || (MouseMovementsEnabled && (Input.mousePosition.x > 0 && Input.mousePosition.x <= ScreenEdgeBorderThickness)))
        {
            movement += Vector2.left;
        }

        if (Input.GetKey(KeyCode.D) || (MouseMovementsEnabled && (Input.mousePosition.x >= (Screen.width - ScreenEdgeBorderThickness))))
        {
            movement += Vector2.right;
        }

        if (movement != Vector2.zero)
        {
            movement.Normalize();
            CamPosition += movement * Speed * CamSize * Time.deltaTime;
        }

        CamSize -= Input.mouseScrollDelta.y * ZoomSpeed;

        if (EnableMovementLimits == true)
        {
            if (SimWorld == null || !SimWorld.HasSingleton<GridInfo>())
                return;

            GridInfo gridRect = SimWorld.GetSingleton<GridInfo>();

            UpdateBounds(gridRect.TileMin, gridRect.TileMax);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            fix3 playerPosition = SimWorld.GetComponentData<FixTranslation>(SimWorldCache.LocalPawn).Value;
            Vector3 cameraPostion = transform.position;
            transform.position = new Vector3((float)playerPosition.x, (float)playerPosition.y, cameraPostion.z);
        }
    }

    private void UpdateBounds(float2 min, float2 max)
    {
        _boundsMin = min;
        _boundsMax = max;
        CamPosition = CamPosition;
    }
}