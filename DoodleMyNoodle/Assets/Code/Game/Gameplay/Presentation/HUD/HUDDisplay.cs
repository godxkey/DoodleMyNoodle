using System;
using UnityEngine;
using UnityEngineX;

public class HUDDisplay : GamePresentationSystem<HUDDisplay>
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _hudContainer;
    [SerializeField] private APEnergyBarDisplay _APEnergyBarDisplay;
    public APEnergyBarDisplay APEnergyBarDisplay => _APEnergyBarDisplay;

    public void SetVisible(bool isVisible)
    {
        _hudContainer.SetActive(isVisible);
    }

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        _canvasGroup.interactable = !GameConsole.IsOpen();
    }
}