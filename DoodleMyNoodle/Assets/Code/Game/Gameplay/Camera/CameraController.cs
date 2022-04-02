using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.MathematicsX;
using UnityEditor;
using CCC.Fix2D;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : GamePresentationSystem<CameraController>
{
    [SerializeField] private Transform _wallpaper;
    [SerializeField] private Vector2 _wallpaperSize = new Vector2(17.778f, 10);

    [Header("Camera Reference")]
    public Camera Cam;

    [Header("Zoom")]
    public float DesiredWidth = 9.5f;

    [Header("Position")]
    public Vector2 OffsetFromGroupPosition = new Vector2(5, 2);

    private Transform _transform;

    public Vector2 CamPosition
    {
        get => _transform.position;
        set => _transform.position = new Vector3(value.x, value.y, _transform.position.z);
    }

    public float CamSize
    {
        get => Cam.orthographicSize;
        set
        {
            Cam.orthographicSize = value;

            if (_wallpaper != null)
            {
                float2 camSize = new float2(value * Screen.width / Screen.height, value);
                float2 minWallpaperScale = camSize / ((float2)_wallpaperSize / 2f);
                _wallpaper.localScale = Vector3.one * max(minWallpaperScale.x, minWallpaperScale.y);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _transform = transform;
    }

    private void LateUpdate()
    {
        CamSize = DesiredWidth * Screen.height / Screen.width / 2f;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        CamPosition = Cache.GroupPosition.ToUnityVec() + OffsetFromGroupPosition;
    }
}