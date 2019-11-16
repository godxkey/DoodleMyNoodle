using System.Collections;
using System.Collections.Generic;


public static class SimulationConstants
{
    // should be equal to Photon-Bolt's "simulation rate" variable, accessible in Bolt>Settings
    public const int TICK_RATE_CONST = 60;

    public static readonly Fix64 TICK_RATE = (Fix64)TICK_RATE_CONST; 
    public static readonly Fix64 TIME_STEP = 1 / TICK_RATE; // must match unity's Fixed timestep
}
