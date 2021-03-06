using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class DoodleLibraryDoodleDisplay : MonoBehaviour
{
    [SerializeField] public RawImage _image;
    [SerializeField] public Button _loadButton;

    private Action<Texture2D> _onDoodleDisplaySelected;

    public void SetDoodleDisplay(Texture2D doodle, Action<Texture2D> onDoodleDisplaySelected)
    {
        _loadButton.onClick.AddListener(OnLoadButtonClicked);
        _image.texture = doodle;
        _onDoodleDisplaySelected = onDoodleDisplaySelected;
    }

    private void OnLoadButtonClicked()
    {
        _onDoodleDisplaySelected.Invoke((Texture2D)_image.texture);
    }
}