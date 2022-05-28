using System;
using UnityEngine;
using UnityEngineX;

public class GameLightingSystem : GamePresentationSystem<GameLightingSystem>
{
    [SerializeField] private GameObject _activatableLight = null;
    private bool _activate = true;

    protected override void Awake()
    {
        base.Awake();

        _activatableLight.SetActive(false);
    }

    public override void PresentationPostSimulationTick()
    {
        base.PresentationPostSimulationTick();
        if (_activate)
        {
            _activatableLight.SetActive(true);
            _activate = false;
        }
    }
}