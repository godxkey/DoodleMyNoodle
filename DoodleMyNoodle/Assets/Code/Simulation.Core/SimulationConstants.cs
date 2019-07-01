using System.Collections;
using System.Collections.Generic;


public static class SimulationConstants
{
    public static readonly Fix64 TICK_RATE = 50; 
    public static readonly Fix64 TIME_STEP = 1 / TICK_RATE; // must match unity's Fixed timestep
}
