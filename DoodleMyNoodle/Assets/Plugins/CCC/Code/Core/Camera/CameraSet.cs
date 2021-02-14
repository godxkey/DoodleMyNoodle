using UnityEngine;

public class CameraSet
{
    public enum DeactivateMode
    {
        DisableComponents,
        DisableGameObject
    }

    public Camera Camera { get; set; }
    public AudioListener AudioListener { get; set; }
    public DeactivateMode Mode { get; private set; }

    public CameraSet(Camera camera, AudioListener audioListener, DeactivateMode deactivateMode)
    {
        Camera = camera; 
        AudioListener = audioListener;
        Mode = deactivateMode;
    }

    public void Activate()
    {
        Camera.gameObject.tag = "MainCamera";
        SetActive(true);
    }

    public void Deactivate()
    {
        Camera.gameObject.tag = "Untagged";
        SetActive(false);
    }

    public void SetActive(bool state)
    {
        switch (Mode)
        {
            case DeactivateMode.DisableComponents:
                if (Camera != null)
                    Camera.enabled = state;
                if (AudioListener != null)
                    AudioListener.enabled = state;
                break;

            case DeactivateMode.DisableGameObject:
                Camera.gameObject.SetActive(state);
                AudioListener.gameObject.SetActive(state); // might be different gameobjects
                break;
        }
    }
}
