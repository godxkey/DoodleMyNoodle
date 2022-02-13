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
    [SerializeField] private float _wallpaperVerticalSize = 10f;

    [Header("Camera Reference")]
    public Camera Cam;

    [Header("Zoom")]
    public float desiredWidth = 1;

    [Header("Position")]
    public Vector2 offsetFromGroupPosition = new Vector2(0,0);

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
                _wallpaper.localScale = Vector3.one * (value / (_wallpaperVerticalSize / 2));
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _transform = transform;
        CamSize = desiredWidth * Screen.height / Screen.width;
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        CamPosition = Cache.GroupPosition.ToUnityVec() + offsetFromGroupPosition;
    }
}