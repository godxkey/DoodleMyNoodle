using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelRegistry
{
    public static DebugPanel[] registeredPanels = new DebugPanel[]
    {
        new DebugPanelGameState(),
        new DebugPanelPlayerRepertoire(),
        new DebugPanelClientSimController(),
        new DebugPanelSimEntities()
    };
}
