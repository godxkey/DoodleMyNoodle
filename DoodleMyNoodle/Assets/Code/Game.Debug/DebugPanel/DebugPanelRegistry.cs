using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelRegistry
{
    public static DebugPanel[] RegisteredPanels = new DebugPanel[]
    {
        new DebugPanelGameState(),
        new DebugPanelPlayerRepertoire(),
        new DebugPanelSimPlayers(),
        new DebugPanelClientSimController(),
        new DebugPanelPlayerAssets(),
    };
}
