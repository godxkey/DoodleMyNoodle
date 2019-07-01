using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static readonly float ONLINE_PACKETS_PER_SECOND = 20; // must match Bolt settings

    public static readonly int EXPECTED_CLIENT_SIM_TICK_QUEUE_LENGTH = 
        ((float)SimulationConstants.TICK_RATE / ONLINE_PACKETS_PER_SECOND).CeiledToInt();

    // For every extra sim tick the player has in its queue, it will play the simulation 10% faster
    public static readonly float CLIENT_SIM_TICK_CATCH_UP_FACTOR = .1f;
}
