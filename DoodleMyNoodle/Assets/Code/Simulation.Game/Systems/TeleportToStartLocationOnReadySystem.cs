using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public class TeleportToStartLocationOnReadySystem : SimComponentSystem
{
    private List<Entity> _players = new List<Entity>();

    protected override void OnUpdate()
    {
        // How many player in game ?
        _players.Clear();

        // count players
        Entities
            .WithAll<PlayerTag, ControlledEntity>()
            .ForEach((Entity player, ref Active isActive) =>
            {
                if (isActive)
                {
                    _players.Add(player);
                }
            });

        // How many player ready ?
        int playerReadyAmount = 0;
        Entities
            .WithAll<Interactable, ReadyInteractableTag>()
            .ForEach((ref Interacted interacted) =>
            {
                if (interacted)
                {
                    playerReadyAmount++;
                }
            });

        // Teleport to start location if not already done
        if ((_players.Count > 0) && (_players.Count == playerReadyAmount) && !HasSingleton<ScenarioHasStartedSingletonTag>())
        {
            // Get Teleport Location
            List<fix2> teleportLocations = new List<fix2>();
            Entities
                .WithAll<StartLocationTag>()
                .ForEach((ref FixTranslation translation) =>
                {
                    teleportLocations.Add(translation);
                });

            if(teleportLocations.Count == 0)
            {
                Log.Warning("No start locations in the level");
                return;
            }

            if (teleportLocations.Count < _players.Count)
            {
                Log.Warning("Not enough start locations in the level. We will reuse some more than once.");
            }

            FixRandom rand = World.Random();
            rand.Shuffle(teleportLocations);

            for (int i = 0; i < _players.Count; i++)
            {
                Entity playerPawn = EntityManager.GetComponentData<ControlledEntity>(_players[i]);
                if (EntityManager.Exists(playerPawn))
                {
                    CommonWrites.RequestTeleport(Accessor, playerPawn, teleportLocations[i % teleportLocations.Count]);
                }
            }

            EntityManager.CreateEntity(typeof(ScenarioHasStartedSingletonTag));
        }
    }
}