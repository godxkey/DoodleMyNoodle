using CCC.Fix2D;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class InitializeGameContantsSystem : SimSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        SimulationGameConstants.InitIfNeeded();
    }
    protected override void OnUpdate() { }
}

public static class SimulationGameConstants
{

    public static readonly fix2 Gravity = new fix2(0, (fix)(-7.45f));
    public static readonly fix InteractibleMaxDistanceManhattan = (fix)1.5;
    public static readonly fix AIShootSpeedIfNoGravity = (fix)3f;
    public static readonly fix AISightDistance = 8;
    public static readonly fix AISightDistanceSq = AISightDistance * AISightDistance;
    public static readonly fix2 AIEyeOffset = new fix2(0, (fix)0.15f);
    public static readonly fix AISearchForPositionMaxCost = (fix)10;
    public static readonly fix AgentRepathCooldown = (fix)1; // repath every 1s
    public static readonly fix AIPauseDurationAfterShoot = (fix)2;

    private static bool s_init = false;

    public static void InitIfNeeded()
    {
        if (s_init)
            return;

        Physics.TerrainFilter.Data = CollisionFilter.FromLayer(Physics.LAYER_TERRAIN);
        Physics.CharactersAndTerrainFilter.Data = CollisionFilter.FromLayers(Physics.LAYER_TERRAIN, Physics.LAYER_CHARACTER);

        s_init = true;
    }

    public class Physics
    {
        public const int LAYER_TERRAIN = 8;
        public const int LAYER_CHARACTER = 9;

        public static readonly SharedStatic<CollisionFilter> TerrainFilter = SharedStatic<CollisionFilter>.GetOrCreate<Physics, TerrainFilterKey>();
        private class TerrainFilterKey { }

        public static readonly SharedStatic<CollisionFilter> CharactersAndTerrainFilter = SharedStatic<CollisionFilter>.GetOrCreate<Physics, CharactersAndTerrainFilterKey>();
        private class CharactersAndTerrainFilterKey { }

    }
}
