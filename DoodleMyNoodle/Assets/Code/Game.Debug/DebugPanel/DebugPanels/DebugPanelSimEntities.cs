using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelSimEntities : DebugPanel
{
    public override string title => "Sim Entities";

    public override bool canBeDisplayed => SimulationBase.IsRunningOrReadyToRun && Game.Started;

    public override void OnGUI()
    {
        foreach (SimEntity entity in SimulationBase.Entities)
        {
            GUILayout.Label(entity.gameObject.name);
        }
    }
}
