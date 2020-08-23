using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;
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
    private DirtyValue<Entity> _localPawn;

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
        CenterOnPawnIfTeleport();
    }

    protected override void OnGamePresentationUpdate() { }

    void Update()
    {
        if (GameConsole.IsOpen())
        {
            return;
        }

        CenterOnPawnIfChange();

        UpdateMovement();

        UpdateSize();

        UpdateBounds();
    }

    private void CenterOnPawnIfTeleport()
    {
        SimWorld.Entities.ForEach((ref TeleportEventData teleportEvent) =>
        {
            // center camera on pawn after teleport
            if (teleportEvent.Entity == SimWorldCache.LocalPawn)
            {
                CamPosition = SimWorldCache.LocalPawnPositionFloat;
            }
        });
    }

    private void CenterOnPawnIfChange()
    {
        // center camera on pawn if pawn change (or on game start)
        _localPawn.Set(SimWorldCache.LocalPawn);
        if (_localPawn.ClearDirty() && _localPawn.Get() != Entity.Null)
        {
            CamPosition = SimWorldCache.LocalPawnPositionFloat;
        }
    }

    private void UpdateMovement()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || (MouseMovementsEnabled && (Input.mousePosition.y >= (Screen.height - ScreenEdgeBorderThickness))))
        {
            movement += Vector2.up;
        }

        if (Input.GetKey(KeyCode.S) || (MouseMovementsEnabled && (Input.mousePosition.y >= 0 && Input.mousePosition.y <= ScreenEdgeBorderThickness)))
        {
            movement -= Vector2.up;
        }

        if (Input.GetKey(KeyCode.A) || (MouseMovementsEnabled && (Input.mousePosition.x >= 0 && Input.mousePosition.x <= ScreenEdgeBorderThickness)))
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

        if (Input.GetKey(KeyCode.Space) && SimWorldCache.LocalPawn != Entity.Null)
        {
            fix3 playerPosition = SimWorld.GetComponentData<FixTranslation>(SimWorldCache.LocalPawn).Value;
            Vector3 cameraPostion = transform.position;
            transform.position = new Vector3((float)playerPosition.x, (float)playerPosition.y, cameraPostion.z);
        }
    }

    private void UpdateSize()
    {
        CamSize -= Input.mouseScrollDelta.y * ZoomSpeed;
    }

    private void UpdateBounds()
    {
        float2 min = float2(float.MinValue, float.MinValue);
        float2 max = float2(float.MaxValue, float.MaxValue);

        if (EnableMovementLimits == true)
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
}