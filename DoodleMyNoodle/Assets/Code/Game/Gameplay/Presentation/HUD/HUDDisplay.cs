using System;
using UnityEngine;
using UnityEngineX;

public class HUDDisplay : GamePresentationSystem<HUDDisplay>
{
    [SerializeField] private GameObject _hudContainer;

    protected override void OnGamePresentationUpdate() { }

    public void ToggleVisibility(bool isVisible)
    {
        _hudContainer.SetActive(isVisible);
    }
}