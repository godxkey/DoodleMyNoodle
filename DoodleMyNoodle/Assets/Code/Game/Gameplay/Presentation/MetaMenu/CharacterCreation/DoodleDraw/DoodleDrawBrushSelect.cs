using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class DoodleDrawBrushSelect : MonoBehaviour
{
    [SerializeField] private float _brushSize = 1;
    [SerializeField] private float _gradient = 1;
    [SerializeField] private bool _isSmall = false;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnBrushSelected);
    }

    private void OnBrushSelected()
    {
        CharacterCreationDoodleDraw.Instance.SetBrush(_brushSize, _gradient, this.GetComponentOnlyInChildren<Image>().sprite, _isSmall);
    }
}