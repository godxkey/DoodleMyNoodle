using UnityEngine;
using System;

public class BackupCamera : MonoBehaviour
{
    [SerializeField] Camera cameraComponent;
    [SerializeField] AudioListener audioListener;

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

            CameraSet = new CameraSet(cameraComponent, audioListener);
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
