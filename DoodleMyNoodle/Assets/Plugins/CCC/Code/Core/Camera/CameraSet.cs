using UnityEngine;

public class CameraSet
{
    public CameraSet(Camera camera, AudioListener audioListener) { Camera = camera; AudioListener = audioListener; }
    public Camera Camera { get; set; }
    public AudioListener AudioListener { get; set; }

    public void Activate()
    {
        SetActive(true);
    }

    public void Deactivate()
    {
        SetActive(false);
    }

    public void SetActive(bool state)
    {
        if(Camera != null)
            Camera.enabled = state;
        if(AudioListener != null)
            AudioListener.enabled = state;
    }
}
