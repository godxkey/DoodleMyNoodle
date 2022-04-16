using System;
using UnityEngine;
using UnityEngineX;

public class GameLightingSystem : GamePresentationSystem<GameLightingSystem>
{
    private bool _activate = true;

    protected override void Awake()
    {
        base.Awake();

        gameObject.SetActive(false);
    }

    public override void PresentationPostSimulationTick()
    {
        base.PresentationPostSimulationTick();
        if (_activate)
        {
            gameObject.SetActive(true);
            _activate = false;
        }
    }
}