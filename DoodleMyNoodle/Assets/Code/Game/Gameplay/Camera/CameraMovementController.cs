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

#if UNITY_EDITOR
    [ConsoleVar("CameraMouseMovementEnabledInEditor", "Enable/Disable the game camera moving when moving the mouse pointer near the edges of the screen.", Save = ConsoleVarAttribute.SaveMode.PlayerPrefs)]
    private static bool s_mouseMovementsEnabledInEditor = false;
#endif

    private static bool MouseMovementsEnabled
    {
        get
        {
#if UNITY_EDITOR
            return s_mouseMovementsEnabledInEditor;
#else
            return true;
#endif
        }
    }


    void Update()
    {
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
            transform.Translate(movement * Speed * Time.deltaTime, Space.World);
        }

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