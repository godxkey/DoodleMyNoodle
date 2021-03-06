using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class DoodleLibraryButton : MonoBehaviour
{
    public GameObject DoodleLibraryWindow;

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (DoodleLibraryWindow != null)
        {
            DoodleLibraryWindow.ToggleActiveState();
        }
    }
}