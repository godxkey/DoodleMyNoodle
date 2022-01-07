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
    public static readonly fix DynamicChestTriggerRadius = (fix)2.5f;
    public static readonly fix2 Gravity = new fix2(0, (fix)(-7.45f));
    public static readonly fix InteractibleMaxDistanceManhattan = (fix)1.5;
    public static readonly fix AIShootSpeedIfNoGravity = (fix)3f;
    public static readonly fix AISightDistance = 8;
    public static readonly fix AISightDistanceSq = AISightDistance * AISightDistance;
    public static readonly fix2 AIEyeOffset = new fix2(0, (fix)0.15f);
    public static readonly fix AISearchForShootPositionMaxCost = (fix)10;
    public static readonly fix AgentRepathCooldown = (fix)1; // repath every 1s
    public static readonly fix AIPauseDurationAfterShoot = (fix)2;
    public static readonly fix AIGrenadierShootDistanceRatio = (fix)0.6; // attemps to shoot at 60% of the distance to target (to account for bomb bounces)
    public static readonly fix AIThinkGlobalCooldown = (fix)0.5;
    public static readonly fix CharacterRadius = (fix)0.45; // do NOT use this constant unless really necessary. It needs to be removed in the future

    private static bool s_init = false;

    public static void InitIfNeeded()
    {
        if (s_init)
            return;

        // For a collision detection to happen between two filters, their 'BelongsTo' and 'CollidesWith' must mention each other

        Physics.CollideWithTerrainFilter.Data = new CollisionFilter()
        {
            BelongsTo = ~(uint)0,
            CollidesWith = CollisionFilter.CreateMask(Physics.LAYER_TERRAIN)
        };
        Physics.CollideWithCharactersAndTerrainFilter.Data = new CollisionFilter()
        {
            BelongsTo = ~(uint)0,
            CollidesWith = CollisionFilter.CreateMask(Physics.LAYER_TERRAIN, Physics.LAYER_CHARACTER)
        };
        Physics.CollideWithCharactersFilter.Data = new CollisionFilter()
        {
            BelongsTo = ~(uint)0,
            CollidesWith = CollisionFilter.CreateMask(Physics.LAYER_CHARACTER)
        };

        s_init = true;
    }

    public class Physics
    {
        public const int LAYER_TERRAIN = 8;
        public const int LAYER_CHARACTER = 9;

        public static readonly SharedStatic<CollisionFilter> CollideWithTerrainFilter = SharedStatic<CollisionFilter>.GetOrCreate<Physics, TerrainFilterKey>();
        private class TerrainFilterKey { }

        public static readonly SharedStatic<CollisionFilter> CollideWithCharactersAndTerrainFilter = SharedStatic<CollisionFilter>.GetOrCreate<Physics, CharactersAndTerrainFilterKey>();
        private class CharactersAndTerrainFilterKey { }

        public static readonly SharedStatic<CollisionFilter> CollideWithCharactersFilter = SharedStatic<CollisionFilter>.GetOrCreate<Physics, CollideWithCharactersFilterKey>();
        private class CollideWithCharactersFilterKey { }

    }
}
