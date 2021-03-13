using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class DoodleLibraryDoodleDisplay : MonoBehaviour
{
    [SerializeField] public RawImage _image;
    [SerializeField] public Button _loadButton;
    [SerializeField] public Button _deleteButton;

    private Action<Texture2D> _onDoodleDisplaySelected;
    private Action<int> _onDoodleDisplayDeleted;

    private int _doodleIndex;

    public void SetDoodleDisplay(Texture2D doodle, int index, Action<Texture2D> onDoodleDisplaySelected, Action<int> onDoodleDisplayDeleted)
    {
        _loadButton.onClick.AddListener(OnLoadButtonClicked);
        _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        _doodleIndex = index;
        _image.texture = doodle;
        _onDoodleDisplaySelected = onDoodleDisplaySelected;
        _onDoodleDisplayDeleted = onDoodleDisplayDeleted;
    }

    private void OnLoadButtonClicked()
    {
        if (_onDoodleDisplaySelected != null)
        {
            _onDoodleDisplaySelected.Invoke((Texture2D)_image.texture);
        }
    }

    private void OnDeleteButtonClicked()
    {
        if (_onDoodleDisplayDeleted != null)
        {
            _onDoodleDisplayDeleted.Invoke(_doodleIndex);
        }
    }
}