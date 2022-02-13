using System;
using UnityEngine;
using UnityEngineX;

public class HUDDisplay : GamePresentationSystem<HUDDisplay>
{
    [SerializeField] private GameObject _hudContainer;
    [SerializeField] private APEnergyBarDisplay _APEnergyBarDisplay;
    public APEnergyBarDisplay APEnergyBarDisplay => _APEnergyBarDisplay;

    protected override void OnGamePresentationUpdate() { }

    public void ToggleVisibility(bool isVisible)
    {
        _hudContainer.SetActive(isVisible);
    }
}