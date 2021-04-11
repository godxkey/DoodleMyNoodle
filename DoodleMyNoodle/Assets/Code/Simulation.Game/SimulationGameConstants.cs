using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimulationGameConstants
{
    public const int TERRAIN_PHYSICS_LAYER = 8;

    public static readonly fix2 Gravity = new fix2(0, (fix)(-7.45f));
    public static readonly fix InteractibleMaxDistanceManhattan = (fix)1.5;
}
