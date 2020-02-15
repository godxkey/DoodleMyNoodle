using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fbesssette TODO: This should be moved out of the simulation

[NetSerializable]
public class SimCommandLoadPresentationScene: SimCommand
{
    public string SceneName;

    public override void Execute()
    {
        Simulation.AddPresentationScene(SceneName);
    }
}