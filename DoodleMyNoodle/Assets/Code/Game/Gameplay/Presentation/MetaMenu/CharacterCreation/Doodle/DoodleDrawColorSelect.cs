using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class DoodleDrawColorSelect : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnColorSelected);
    }

    private void OnColorSelected()
    {
        CharacterCreationDoodleDraw.Instance.SetBrushColor(GetComponent<Image>().color);
    }
}