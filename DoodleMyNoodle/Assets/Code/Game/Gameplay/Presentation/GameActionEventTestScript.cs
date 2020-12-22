using System;
using UnityEngine;
using UnityEngineX;

public class GameActionEventTestScript : GamePresentationSystem<GameActionEventTestScript>
{
    public override void OnPostSimulationTick()
    {
        Cache.SimWorld.Entities.ForEach((ref GameActionEventData gameActionEventData) =>
        {
            Debug.Log("Game Action has been triggered");
        });
    }

    protected override void OnGamePresentationUpdate() { }
}