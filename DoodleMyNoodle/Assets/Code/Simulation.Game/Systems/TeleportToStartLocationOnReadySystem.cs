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
        int playerAmount = 0;
        Entities
        .WithAll<PlayerTag>()
        .ForEach((ref Active isActive) =>
        {
            if (isActive)
            {
                playerAmount++;
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
        if ((playerAmount > 0) && (playerAmount == playerReadyAmount) && !HasSingleton<ScenarioHasStarted>())
        {
            // Get Teleport Location
            List<fix3> teleportLocation = new List<fix3>();
            Entities
            .WithAll<StartLocation>()
            .ForEach((ref FixTranslation translation) =>
            {
                teleportLocation.Add(translation.Value);
            });

            if (teleportLocation.Count < playerAmount)
            {
                Log.Error("Not Enough Start Location in the level");
                return;
            }

            int index = 0;
            teleportLocation.Shuffle();
            Entities
            .WithAll<ControllableTag>()
            .ForEach((Entity entity, ref FixTranslation translation) =>
            {
                EntityManager.SetComponentData(entity, new FixTranslation() { Value = teleportLocation[index] });
                index++;
            });

            EntityManager.CreateEntity(typeof(ScenarioHasStarted));
        }
    }
}