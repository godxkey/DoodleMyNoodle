using UnityEngine;
using System;
using CCC.InspectorDisplay;
using UnityEngine.Serialization;

public class BackupCamera : MonoBehaviour
{
    [FormerlySerializedAs("cameraComponent")]
    [AutoFetch, SerializeField] private Camera _cameraComponent;

    [FormerlySerializedAs("audioListener")]
    [AutoFetch, SerializeField] private AudioListener _audioListener;

    [FormerlySerializedAs("deactivateMode")]
    [SerializeField] private CameraSet.DeactivateMode _deactivateMode = CameraSet.DeactivateMode.DisableGameObject;

    public CameraSet CameraSet { get; private set; }
    public Camera Camera { get { return _cameraComponent; } }
    public AudioListener AudioListener { get { return _audioListener; } }

    static public BackupCamera Instance { get; set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            CameraSet = new CameraSet(_cameraComponent, _audioListener, _deactivateMode);
        }
    }

    private void Update()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movement += Vector2.up;
        }

        if (Input.GetKey(KeyCode.S))
        {
            movement -= Vector2.up;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movement += Vector2.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement += Vector2.right;
        }

        if (movement != Vector2.zero)
        {
            movement.Normalize();
            Vector2 displacement = movement * _cameraComponent.orthographicSize * Time.deltaTime;
            transform.position += new Vector3(displacement.x, displacement.y, 0);
        }
    }

    void OnDestroy()
    {
        if (ReferenceEquals(Instance, this))
        {
            Instance = null;
        }
    }
}
