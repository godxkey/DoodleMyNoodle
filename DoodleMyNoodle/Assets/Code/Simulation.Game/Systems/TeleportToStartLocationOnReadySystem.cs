using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public class TeleportToStartLocationOnReadySystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // How many player in game ?
        List<Entity> players = new List<Entity>();
        Entities
        .WithAll<PlayerTag>()
        .ForEach((Entity player, ref Active isActive) =>
        {
            if (isActive)
            {
                players.Add(player);
            }
        });

        // How many player ready ?
        int playerReadyAmount = 0;
        Entities
        .WithAll<Interactable, ReadyInteractableTag>()
        .WithNone<StartingInventoryItem>()
        .ForEach((ref Interacted interacted) =>
        {
            if (interacted)
            {
                playerReadyAmount++;
            }
        });

        // Teleport to start location if not already done
        if ((players.Count > 0) && (players.Count == playerReadyAmount) && !HasSingleton<ScenarioHasStarted>())
        {
            // Get Teleport Location
            List<fix3> teleportLocation = new List<fix3>();
            Entities
            .WithAll<StartLocation>()
            .ForEach((ref FixTranslation translation) =>
            {
                teleportLocation.Add(translation.Value);
            });

            if (teleportLocation.Count < players.Count)
            {
                Log.Error("Not Enough Start Location in the level");
                return;
            }

            FixRandom Random = World.Random();
            Random.Shuffle(teleportLocation);

            for (int i = 0; i < players.Count; i++)
            {
                Entity playerPawn = EntityManager.GetComponentData<ControlledEntity>(players[i]).Value;
                if (playerPawn != Entity.Null)
                {
                    EntityManager.SetComponentData(playerPawn, new FixTranslation() { Value = teleportLocation[i] });
                }
            }

            EntityManager.CreateEntity(typeof(ScenarioHasStarted));
        }
    }
}