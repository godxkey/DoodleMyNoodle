using UnityEngine;
using System;
using CCC.InspectorDisplay;

public class BackupCamera : MonoBehaviour
{
    [AutoFetch, SerializeField] private Camera cameraComponent;
    [AutoFetch, SerializeField] private AudioListener audioListener;
    [SerializeField] private CameraSet.DeactivateMode deactivateMode = CameraSet.DeactivateMode.DisableGameObject;

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

    void OnDestroy()
    {
        if(ReferenceEquals(Instance, this))
        {
            Instance = null;
        }
    }
}
