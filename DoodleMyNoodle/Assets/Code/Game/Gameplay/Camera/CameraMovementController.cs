using Unity.MathematicsX;
using UnityEditor;
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

    void Update()
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
            ExternalSimWorldAccessor simWorld = GameMonoBehaviourHelpers.GetSimulationWorld();

            if (simWorld == null || !simWorld.HasSingleton<GridInfo>())
                return;

            GridInfo gridRect = simWorld.GetSingleton<GridInfo>();

            Vector3 cameraPosition = transform.position;
            transform.position = new Vector3(Mathf.Clamp(cameraPosition.x, gridRect.TileMin.x, gridRect.TileMax.x),
                                             Mathf.Clamp(cameraPosition.y, gridRect.TileMin.y, gridRect.TileMax.y),
                                             cameraPosition.z);
        }
    }
}