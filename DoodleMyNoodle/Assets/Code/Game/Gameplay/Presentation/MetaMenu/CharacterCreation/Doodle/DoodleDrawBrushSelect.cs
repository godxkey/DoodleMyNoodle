using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class DoodleDrawBrushSelect : MonoBehaviour
{
    [SerializeField] private float _brushSize = 1;
    [SerializeField] private float _gradient = 1;
    [SerializeField] private bool _isSmall = false;

    [SerializeField] private bool _defaultBrush = false;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnBrushSelected);

        if (_defaultBrush)
        {
            OnBrushSelected();
            transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnBrushSelected()
    {
        CharacterCreationDoodleDraw.Instance.SetBrush(_brushSize, _gradient, this.GetComponentOnlyInChildren<Image>().sprite, _isSmall);
    }
}