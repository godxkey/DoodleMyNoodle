using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject CameraA;
    public GameObject CameraB;
    public GameObject CameraC;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7))
            CameraService.Instance.AddCamera(CameraA.GetComponent<Camera>(), CameraA.GetComponent<AudioListener>(), 0);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            CameraService.Instance.AdjustCameraPriority(CameraA.GetComponent<Camera>(), 10);
        if (Input.GetKeyDown(KeyCode.Keypad1))
            CameraService.Instance.RemoveCamera(CameraA.GetComponent<Camera>());

        if (Input.GetKeyDown(KeyCode.Keypad8))
            CameraService.Instance.AddCamera(CameraB.GetComponent<Camera>(), CameraB.GetComponent<AudioListener>(), 1);
        if (Input.GetKeyDown(KeyCode.Keypad5))
            CameraService.Instance.AdjustCameraPriority(CameraB.GetComponent<Camera>(), 11);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            CameraService.Instance.RemoveCamera(CameraB.GetComponent<Camera>());

        if (Input.GetKeyDown(KeyCode.Keypad9))
            CameraService.Instance.AddCamera(CameraC.GetComponent<Camera>(), CameraC.GetComponent<AudioListener>(), 2);
        if (Input.GetKeyDown(KeyCode.Keypad6))
            CameraService.Instance.AdjustCameraPriority(CameraC.GetComponent<Camera>(), 12);
        if (Input.GetKeyDown(KeyCode.Keypad3))
            CameraService.Instance.RemoveCamera(CameraC.GetComponent<Camera>());
    }
}
