using CCC.Fix2D;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class InitializeGameContantsSystem : SimGameSystemBase
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
    public static readonly fix2 Gravity = new fix2(0, (fix)(-7.45f)); // since player characters are about 0.9 units in size, this is the realistic gravity
    public static readonly fix InteractibleMaxDistanceManhattan = (fix)1.5;
    public static readonly fix CharacterRadius = (fix)0.45; // do NOT use this constant unless really necessary. It needs to be removed in the future

    public static readonly fix SameEffectGroupDamageCooldown = (fix)1; // do NOT use this constant unless really necessary. It needs to be removed in the future

    public static readonly int FallDamage = 1;

    /// <summary>
    /// Minimal impulse required for fall damage on entities that have this footing: NO COMPONENT
    /// </summary>
    public static readonly fix ImpulseThresholdFallDamageNonNavAgents = 2;

    /// <summary>
    /// Minimal impulse required for fall damage on entities that have this footing: AirControl
    /// </summary>
    public static readonly fix ImpulseThresholdFallDamageAirControl = 4;

    /// <summary>
    /// Minimal impulse required for fall damage on entities that have this footing: None
    /// </summary>
    public static readonly fix ImpulseThresholdFallDamageNoAirControl = 3;

    /// <summary>
    /// Minimal impulse required for instigator to destroy victim tile
    /// </summary>
    public static readonly fix ImpulseThresholdDestroyingTile = 5;

    public static readonly fix FallDamageCooldown = (fix)0.6;
    public static readonly fix OutOfBoundsLeftDistanceFromPlayerGroup = 5;
    public static readonly fix OutOfBoundsRightDistanceFromPlayerGroup = 15;
    public static readonly fix EnemySpawnDistanceFromPlayerGroup = 15;
    public static readonly fix DeadEntityDestroyDelay = 3;
    public static readonly fix DisabledEntityDestroyDelay = 3;
    public static readonly fix UnspawnedMobsMoveSpeed = -(fix)0.5; // this should ideally matcht the base move speed of regular mobs
    public static readonly fix GroundedBackMaxSpeed = (fix)0.01; // the max velocity the ungrounded actor can have to be grounded again
    public static readonly fix ActorAcceleration = (fix)7.5; // the acceleration of the actors to reach their desired MoveSpeed


    private static bool s_init = false;

    public static void InitIfNeeded()
    {
        if (s_init)
            return;

        // For a collision detection to happen between two filters, their 'BelongsTo' and 'CollidesWith' must mention each other

        Physics.CharacterFilter.Data = CollisionFilter.FromLayer(Physics.LAYER_CHARACTER);

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
        public const int LAYER_CORPSES = 13;

        public static readonly SharedStatic<CollisionFilter> CharacterFilter = SharedStatic<CollisionFilter>.GetOrCreate<Physics, CharacterFilterKey>();
        private class CharacterFilterKey { }

        public static readonly SharedStatic<CollisionFilter> CollideWithTerrainFilter = SharedStatic<CollisionFilter>.GetOrCreate<Physics, ColliderWithTerrainFilterKey>();
        private class ColliderWithTerrainFilterKey { }

        public static readonly SharedStatic<CollisionFilter> CollideWithCharactersAndTerrainFilter = SharedStatic<CollisionFilter>.GetOrCreate<Physics, CollideWithCharactersAndTerrainFilterKey>();
        private class CollideWithCharactersAndTerrainFilterKey { }

        public static readonly SharedStatic<CollisionFilter> CollideWithCharactersFilter = SharedStatic<CollisionFilter>.GetOrCreate<Physics, CollideWithCharactersFilterKey>();
        private class CollideWithCharactersFilterKey { }

    }
}

public partial struct Helpers
{
    public static bool BelongsToTerrain(CollisionFilter colliderFilter) 
        => (colliderFilter.BelongsTo & (1 << SimulationGameConstants.Physics.LAYER_TERRAIN)) != 0;
}
