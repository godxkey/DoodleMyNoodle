using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelGameState : DebugPanel
{
    public override string title => "Game State";
    public override bool canBeDisplayed => true; // this debug panel can always be displayed as we always have a GameState

    public override void OnGUI()
    {
        GUILayout.Label("current: " + GameStateManager.currentGameState);
        GameStateManager.currentGameState?.OnDebugPanelGUI();
    }
}
