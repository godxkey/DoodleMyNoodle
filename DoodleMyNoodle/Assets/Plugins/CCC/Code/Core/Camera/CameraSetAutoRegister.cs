using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetAutoRegister : MonoBehaviour
{
    [SerializeField] RegisterOptions registerSetting;
    [SerializeField] UnregisterOptions unregisterSetting;
    [SerializeField] new Camera camera;
    [SerializeField] AudioListener audioListener;
    [SerializeField] int priority;

    bool registered;
    Action onCoreServiceInitComplete;

    private enum RegisterOptions
    {
        OnAwake,
        OnStart,
        OnEnable
    }
    private enum UnregisterOptions
    {
        OnDestroy,
        OnDisable,
    }

    public void Register()
    {
        if (registered == false)
        {
            registered = true;
            CameraService.Instance.AddCamera(camera, audioListener, priority);
        }
    }

    public void Unregister()
    {
        if (registered)
        {
            registered = false;
            CameraService.Instance.RemoveCamera(camera);
        }
    }

    void Awake()
    {
        if (registerSetting == RegisterOptions.OnAwake)
            RegisterButWaitForCamerService();
    }

    void Start()
    {
        if (registerSetting == RegisterOptions.OnStart)
            RegisterButWaitForCamerService();
    }

    void OnEnable()
    {
        if (registerSetting == RegisterOptions.OnEnable)
            RegisterButWaitForCamerService();
    }

    void OnDisable()
    {
        if (unregisterSetting == UnregisterOptions.OnDisable && ApplicationUtilityService.ApplicationIsQuitting == false)
            UnregisterAndCancelCallbackIfNecessary();
    }

    void OnDestroy()
    {
        if (unregisterSetting == UnregisterOptions.OnDestroy && ApplicationUtilityService.ApplicationIsQuitting == false)
            UnregisterAndCancelCallbackIfNecessary();
    }

    void OnCoreServiceManagerInitComplete()
    {
        onCoreServiceInitComplete = null;
        Register();
    }

    void RegisterButWaitForCamerService()
    {
        if (CameraService.Instance == null) // we can't register if the camera service is not up and running
        {
            onCoreServiceInitComplete = OnCoreServiceManagerInitComplete;
            CoreServiceManager.AddInitializationCallback<CameraService>(onCoreServiceInitComplete);
        }
        else
        {
            Register();
        }
    }

    void UnregisterAndCancelCallbackIfNecessary()
    {
        if (onCoreServiceInitComplete != null)
        {
            CoreServiceManager.RemoveInitializationCallback<CameraService>(onCoreServiceInitComplete);
        }

        if (CameraService.Instance != null)
        {
            Unregister();
        }
    }
}
