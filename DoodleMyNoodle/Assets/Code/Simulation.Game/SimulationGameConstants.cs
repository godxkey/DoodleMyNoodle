using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimulationGameConstants
{
    public const int PHYSICS_LAYER_TERRAIN = 8;
    public const int PHYSICS_LAYER_CHARACTER = 9;

    public static readonly fix2 Gravity = new fix2(0, (fix)(-7.45f));
    public static readonly fix InteractibleMaxDistanceManhattan = (fix)1.5;
    public static readonly fix AISightDistance = 8;
    public static readonly fix AISightDistanceSq = AISightDistance * AISightDistance;
    public static readonly fix2 AIEyeOffset = new fix2(0, (fix)0.15f);
    public static readonly fix AISearchForPositionMaxCost = (fix)10;
}
