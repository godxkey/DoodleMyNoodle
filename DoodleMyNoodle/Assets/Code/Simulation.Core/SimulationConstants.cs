using System.Collections;
using System.Collections.Generic;


public static class SimulationConstants
{
    // should be equal to Photon-Bolt's "simulation rate" variable, accessible in Bolt>Settings
    public const int TICK_RATE_CONST = 60;

    public static readonly float TICK_RATE_F = TICK_RATE_CONST; 
    public static readonly float TIME_STEP_F = 1f / TICK_RATE_F; // must match unity's Fixed timestep

    public static readonly fix TICK_RATE = (fix)TICK_RATE_CONST; 
    public static readonly fix TIME_STEP = 1 / TICK_RATE; // must match unity's Fixed timestep

    
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
