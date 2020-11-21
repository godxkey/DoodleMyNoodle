using UnityEngine;
using System;
using CCC.InspectorDisplay;

public class BackupCamera : MonoBehaviour
{
    [AutoFetch, SerializeField] private Camera cameraComponent;
    [AutoFetch, SerializeField] private AudioListener audioListener;
    [SerializeField] private CameraSet.DeactivateMode deactivateMode = CameraSet.DeactivateMode.DisableGameObject;
    [SerializeField] private bool enableMovements = false;

    public CameraSet CameraSet { get; private set; }
    public Camera Camera { get { return cameraComponent; } }
    public AudioListener AudioListener { get { return audioListener; } }

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

            CameraSet = new CameraSet(cameraComponent, audioListener, deactivateMode);
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
            Vector2 displacement = movement * cameraComponent.orthographicSize * Time.deltaTime;
            transform.position += new Vector3(displacement.x, displacement.y, 0);
        }
    }

    void OnDestroy()
    {
        if(ReferenceEquals(Instance, this))
        {
            Instance = null;
        }
    }
}
