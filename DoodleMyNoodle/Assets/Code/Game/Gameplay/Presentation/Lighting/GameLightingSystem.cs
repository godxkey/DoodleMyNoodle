using System;
using UnityEngine;
using UnityEngineX;

public class GameLightingSystem : GamePresentationSystem<GameLightingSystem>
{
    public override bool SystemReady => true;

    private bool _activate = true;

    protected override void Awake()
    {
        base.Awake();

        gameObject.SetActive(false);
    }

    public override void OnPostSimulationTick()
    {
        base.OnPostSimulationTick();
        if (_activate)
        {
            gameObject.SetActive(true);
            _activate = false;
        }
    }

    protected override void OnGamePresentationUpdate() { }
}