using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelGameState : DebugPanel
{
    public override string Title => "Game State";
    public override bool CanBeDisplayed => true; // this debug panel can always be displayed as we always have a GameState

    public override void OnGUI()
    {
        GUILayout.Label("current: " + GameStateManager.currentGameState);
        GameStateManager.currentGameState?.OnDebugPanelGUI();
    }
}
