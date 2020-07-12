using System;
using UnityEngine;
using UnityEngineX;

public class HighlightContourDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _outline;
    [SerializeField] private bool _startDisplayed = false;

    private bool _isDisplayed = false;

    private void Awake()
    {
        _isDisplayed = _startDisplayed;
    }

    void Update()
    {
        _outline.SetActive(_isDisplayed);
    }

    public void ChangeVisibility(bool isVisible)
    {
        _isDisplayed = isVisible;
    }
}