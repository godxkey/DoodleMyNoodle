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

    private void Start()
    {
        if (_library != null)
        {
            List<Texture2D> doodles = _library.GetAllDoodles();

            foreach (Texture2D texture in doodles)
            {
                GameObject newDoodleDisplay = Instantiate(_doodleDisplayPrefab, _doodleDisplayContainer);
                if ((newDoodleDisplay != null) && (newDoodleDisplay.GetComponent<DoodleLibraryDoodleDisplay>() != null))
                {
                    newDoodleDisplay.GetComponent<DoodleLibraryDoodleDisplay>().SetDoodleDisplay(texture, OnDoodleSelected);
                }
            }
        }
    }

    private void OnDoodleSelected(Texture2D doodle)
    {
        _doodleDraw.SetCurrentDoodle(doodle);
        gameObject.ToggleActiveState();
    }
}