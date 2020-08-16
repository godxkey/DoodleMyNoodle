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

    private bool _hasTeleportedToPlayerOnGameStart = false;
    private bool _hasTeleportedToPlayerOnGameBegin = false;

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

    protected override void OnGamePresentationUpdate()
    {
        if (GameConsole.IsOpen())
        {
            return;
        }

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || (MouseMovementsEnabled && (Input.mousePosition.y >= (Screen.height - ScreenEdgeBorderThickness))))
        {
            movement += Vector3.up;
        }

        if (Input.GetKey(KeyCode.S) || (MouseMovementsEnabled && (Input.mousePosition.y > 0 && Input.mousePosition.y <= ScreenEdgeBorderThickness)))
        {
            movement -= Vector3.up;
        }

        if (Input.GetKey(KeyCode.A) || (MouseMovementsEnabled && (Input.mousePosition.x > 0 && Input.mousePosition.x <= ScreenEdgeBorderThickness)))
        {
            movement += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D) || (MouseMovementsEnabled && (Input.mousePosition.x >= (Screen.width - ScreenEdgeBorderThickness))))
        {
            movement += Vector3.right;
        }

        if (movement != Vector3.zero)
        {
            movement.Normalize();
            transform.Translate(movement * Speed * Cam.orthographicSize * Time.deltaTime, Space.World);
        }

        Cam.orthographicSize -= Input.mouseScrollDelta.y * ZoomSpeed;
        Cam.orthographicSize = Mathf.Clamp(Cam.orthographicSize, 1, MaxZoom);

        if (EnableMovementLimits == true)
        {
            if (SimWorld.TryGetSingleton(out GridInfo gridInfo))
            {
                intRect gridRect = gridInfo.GridRect;

                Vector3 cameraPostion = transform.position;
                transform.position = new Vector3(Mathf.Clamp(cameraPostion.x, gridRect.min.x, gridRect.max.x),
                                                 Mathf.Clamp(cameraPostion.y, gridRect.min.y, gridRect.max.y),
                                                 cameraPostion.z);
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            fix3 playerPosition = SimWorld.GetComponentData<FixTranslation>(SimWorldCache.LocalPawn).Value;
            Vector3 cameraPostion = transform.position;
            transform.position = new Vector3((float)playerPosition.x, (float)playerPosition.y, cameraPostion.z);
        }

        if (SimWorld.HasSingleton<GameStartedTag>() && !_hasTeleportedToPlayerOnGameStart)
        {
            _hasTeleportedToPlayerOnGameStart = true;

            fix3 playerPosition = SimWorld.GetComponentData<FixTranslation>(SimWorldCache.LocalPawn).Value;
            Vector3 cameraPostion = transform.position;
            transform.position = new Vector3((float)playerPosition.x, (float)playerPosition.y, cameraPostion.z);
        }

        // todo after starting zone teleport camera here
    }
}