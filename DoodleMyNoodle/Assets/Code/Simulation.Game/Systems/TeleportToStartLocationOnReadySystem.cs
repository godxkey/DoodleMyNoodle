using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using Unity.Collections;

public class TeleportToStartLocationOnReadySystem : SimGameSystemBase
{
    private NativeList<Entity> _players;

    protected override void OnCreate()
    {
        base.OnCreate();
        _players = new NativeList<Entity>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _players.Dispose();
    }

    protected override void OnUpdate()
    {
        // How many player in game ?
        _players.Clear();

        // count active players
        var players = _players;
        Entities
            .WithAll<PlayerTag, ControlledEntity>()
            .ForEach((Entity player, in Active isActive) =>
            {
                if (isActive)
                {
                    players.Add(player);
                }
            }).Run();

        // How many player ready ?
        int playerReadyAmount = 0;
        Entities
            .WithAll<ReadyToPlayTag>()
            .ForEach((in Signal signal) =>
            {
                if (signal)
                {
                    playerReadyAmount++;
                }
            }).Run();

        // Teleport to start location if not already done
        if ((players.Length > 0) && (players.Length == playerReadyAmount) && !HasSingleton<ScenarioHasStartedSingletonTag>())
        {
            // Get Teleport Location
            NativeList<fix2> teleportLocations = new NativeList<fix2>(Allocator.TempJob);
            Entities
                .WithAll<StartLocationTag>()
                .ForEach((in FixTranslation translation) =>
                {
                    teleportLocations.Add(translation);
                }).Run();

            if(teleportLocations.Length == 0)
            {
                Log.Warning("No start locations in the level");
                return;
            }

            if (teleportLocations.Length < players.Length)
            {
                Log.Warning("Not enough start locations in the level. We will reuse some more than once.");
            }

            FixRandom rand = World.Random();
            rand.Shuffle(teleportLocations);

            for (int i = 0; i < players.Length; i++)
            {
                Entity playerPawn = EntityManager.GetComponentData<ControlledEntity>(players[i]);
                if (EntityManager.Exists(playerPawn))
                {
                    CommonWrites.RequestTeleport(Accessor, playerPawn, teleportLocations[i % teleportLocations.Length]);
                }
            }

            EntityManager.CreateEntity(typeof(ScenarioHasStartedSingletonTag));

            teleportLocations.Dispose();
        }
    }
}