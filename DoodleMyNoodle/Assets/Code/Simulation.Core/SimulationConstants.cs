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

    // In theory, ticks should only stay in queue for this duration: SimTimeStep * ceil((TickRate / PacketsPerSeconds) - 1);
    // 0.01667 * ceil(60/20 - 1)
    // 0.01667 * ceil(3 - 1)
    // 0.01667 * 2
    // 0.03333
    // But in practice, the packet rate varies a lot so ticks should stay in queue a bit longer to ensure a smoother playback
    public static readonly float CLIENT_SIM_TICK_QUEUE_DURATION_MIN = 0.03333f; // minimum queue buffer duration
    public static readonly float CLIENT_SIM_TICK_QUEUE_DURATION_MAX = 0.20000f; // maxiumu queue buffer duration
    public static readonly float CLIENT_SIM_TICK_QUEUE_DURATION_ADJUSTMENT_DELTA = 0.005f; // how much should we adjust the queue buffer duration, per frame

    // How many ticks do we use for interval sampling.
    public static readonly int CLIENT_SIM_TICK_QUEUE_DURATION_SAMPLE_SIZE = 120; 

    // The longest realistic interval between two packets. 
    // Beyond that, we consider it a server lag spike and cap the interval to prevent over-adjustment of the buffer duration.
    public static readonly double CLIENT_SIM_TICK_QUEUE_DURATION_MAX_CONSIDERED_INTERVAL = 0.4d;
}
