using Unity.MathematicsX;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovementController : GameMonoBehaviour
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

    [Header("Debug Options")]
    public bool DisableMouseControlsInEditor = true;

    private bool _shouldDisableMouseMovements = false;

    void Start()
    {
#if UNITY_EDITOR
        _shouldDisableMouseMovements = DisableMouseControlsInEditor;
#endif
    }

    void Update()
    {
        Vector3 _panMovement = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || (!_shouldDisableMouseMovements && (Screen.height <= Screen.height && Input.mousePosition.y >= (Screen.height - ScreenEdgeBorderThickness))))
        {
            _panMovement += Vector3.up * Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S) || (!_shouldDisableMouseMovements && (Input.mousePosition.y > 0 && Input.mousePosition.y <= ScreenEdgeBorderThickness)))
        {
            _panMovement -= Vector3.up * Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A) || (!_shouldDisableMouseMovements && (Input.mousePosition.x > 0 && Input.mousePosition.x <= ScreenEdgeBorderThickness)))
        {
            _panMovement += Vector3.left * Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D) || (!_shouldDisableMouseMovements && (Screen.height <= Screen.width && Input.mousePosition.x >= (Screen.width - ScreenEdgeBorderThickness))))
        {
            _panMovement += Vector3.right * Speed * Time.deltaTime;
        }

        transform.Translate(_panMovement, Space.World);

        Cam.orthographicSize -= Input.mouseScrollDelta.y * ZoomSpeed;
        Cam.orthographicSize = Mathf.Clamp(Cam.orthographicSize, 1, MaxZoom);

        if (EnableMovementLimits == true)
        {
            ExternalSimWorldAccessor simWorld = GameMonoBehaviourHelpers.GetSimulationWorld();

            if (simWorld == null || !simWorld.HasSingleton<GridInfo>())
                return;

            intRect gridRect = simWorld.GetSingleton<GridInfo>().GridRect;

            Vector2 bottomLeft = new Vector2(gridRect.min.x, gridRect.min.y);
            Vector2 bottomRight = new Vector2(gridRect.max.x, gridRect.min.y);
            Vector2 TopLeft = new Vector2(gridRect.min.x, gridRect.max.y);
            Vector2 TopRight = new Vector2(gridRect.max.x, gridRect.max.y);

            Vector3 cameraPostion = transform.position;
            transform.position = new Vector3(Mathf.Clamp(cameraPostion.x, gridRect.min.x, gridRect.max.x), 
                                             Mathf.Clamp(cameraPostion.y, gridRect.min.y, gridRect.max.y), 
                                             cameraPostion.z);
        }
    }
}