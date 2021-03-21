using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static readonly float ONLINE_PACKETS_PER_SECOND = 20; // must match Bolt settings
    public static readonly float ONLINE_PACKET_TIME_INTERVAL = 1f / ONLINE_PACKETS_PER_SECOND;


    public const string LAYER_GRID_BACKGROUND = "Grid_Background";
    public const string LAYER_GRID_SIMULATION = "Grid_Gameplay";
    public const string LAYER_GRID_FOREGROUND = "Grid_Foreground";
}
