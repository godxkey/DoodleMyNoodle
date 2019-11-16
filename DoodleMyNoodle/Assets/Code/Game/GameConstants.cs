using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static readonly float ONLINE_PACKETS_PER_SECOND = 20; // must match Bolt settings
    public static readonly float ONLINE_PACKET_TIME_INTERVAL = 1f / ONLINE_PACKETS_PER_SECOND;


    public static readonly float CLIENT_SIM_TICK_MAX_CATCH_UP_SPEED = 3f;


    /// <summary>
    /// AUGMENTER cette variable augmente la stabilité du 'playback' de la simulation. Ce qui veux dire qu'elle parait plus fluide
    /// <para/>
    /// DIMINUER cette variable augmente la 'responsiveness' de la simulation. Les ticks reçu par le serveur passe moins de temps en file d'attente
    /// </summary>
    public static readonly float CLIENT_SIM_TICK_MAX_EXPECTED_TIME_IN_QUEUE = 0.2f; // devrais être entre 0.03333 et genre 0.1
        //(float)SimulationConstants.TIME_STEP * Mathf.Ceil(((float)SimulationConstants.TICK_RATE / ONLINE_PACKETS_PER_SECOND) - 1);
        // 0.01667 * ceil(60/20 - 1)
        // 0.01667 * ceil(3 - 1)
        // 0.01667 * 2
        // 0.03333
}
