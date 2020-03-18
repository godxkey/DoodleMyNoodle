using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public class CreatePlayerSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        foreach (var input in SimInputs)
        {
            if (input is SimInputPlayerCreate createPlayerInput)
            {
                var newPlayerEntity = EntityManager.CreateEntity(
                    typeof(PlayerTag),
                    typeof(PersistentId),
                    typeof(Name),
                    typeof(ControlledEntity));

                // set persistent id
                EntityManager.SetComponentData(newPlayerEntity, this.MakeUniquePersistentId());

                // cap player name at 30 characters
                string playerName = createPlayerInput.PlayerName;
                if (playerName.Length > 30)
                    playerName = playerName.Substring(0, 30);

                // set name
                EntityManager.SetComponentData(newPlayerEntity, new Name() { Value = playerName });

                // FOR DEBUGGING ONLY
#if UNITY_EDITOR
                EntityManager.SetName(newPlayerEntity, $"Player({playerName})");
#endif
            }
        }
    }
}
