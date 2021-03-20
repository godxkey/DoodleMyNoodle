using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class DoodleLibraryWindow : MonoBehaviour
{
    [SerializeField] private DoodleLibrary _library;
    [SerializeField] private CharacterCreationDoodleDraw _doodleDraw;
    [SerializeField] private Transform _doodleDisplayContainer;
    [SerializeField] private GameObject _doodleDisplayPrefab;

    private List<GameObject> _doodleDisplays = new List<GameObject>();

    private void Start()
    {
        if (_library != null)
        {
            List<Texture2D> doodles = _library.GetAllDoodles();

            for (int i = 0; i < doodles.Count; i++)
            {
                Texture2D texture = doodles[i];
                GameObject newDoodleDisplay = Instantiate(_doodleDisplayPrefab, _doodleDisplayContainer);
                if ((newDoodleDisplay != null) && (newDoodleDisplay.GetComponent<DoodleLibraryDoodleDisplay>() != null))
                {
                    newDoodleDisplay.GetComponent<DoodleLibraryDoodleDisplay>().SetDoodleDisplay(texture, i, OnDoodleSelected, OnDoodleDeleted);
                    _doodleDisplays.Add(newDoodleDisplay);
                }
            }
        }
    }

    private void OnDoodleSelected(Texture2D doodle, int doodleIndex)
    {
        _doodleDraw.SetCurrentDoodle(doodle);
        _library.SetLastDoodle(doodleIndex);
        gameObject.ToggleActiveState();
    }

    private void OnDoodleDeleted(int index)
    {
        _doodleDisplays[index].Destroy();
        _library.RemoveDoodle(index);
        _doodleDraw.EraseCurrentDoodle();
        gameObject.ToggleActiveState();
    }
}