using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static readonly float ONLINE_PACKETS_PER_SECOND = 20; // must match Bolt settings
    public static readonly float ONLINE_PACKET_TIME_INTERVAL = 1f / ONLINE_PACKETS_PER_SECOND;


    public static readonly float CLIENT_SIM_TICK_MAX_CATCH_UP_SPEED = 3f;
    public static readonly float CLIENT_SIM_TICK_MAX_EXPECTED_TIME_IN_QUEUE =
        (float)SimulationConstants.TIME_STEP * Mathf.Ceil(((float)SimulationConstants.TICK_RATE / ONLINE_PACKETS_PER_SECOND) - 1);
        // 0.02 * ceil(50/20 - 1)
        // 0.02 * ceil(2.5 - 1)
        // 0.02 * 2
        // 0.04
}
