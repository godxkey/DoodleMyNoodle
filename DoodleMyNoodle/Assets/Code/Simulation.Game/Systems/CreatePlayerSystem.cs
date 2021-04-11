using Unity.Entities;
using Unity.Collections;
using UnityEngineX;
using Unity.Mathematics;
using Unity.MathematicsX;
using CCC.Fix2D;

[NetSerializable]
public class SimInputPlayerCreate : SimMasterInput
{
    public string PlayerName;
}

[AlwaysUpdateSystem]
public class CreatePlayerSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        foreach (var input in World.TickInputs)
        {
            if (input is SimInputPlayerCreate createPlayerInput)
            {

                var newPlayerEntity = EntityManager.CreateEntity(
                    typeof(PlayerTag),
                    typeof(PersistentId),
                    typeof(Name),
                    typeof(ControlledEntity),
                    typeof(Team),
                    typeof(ReadyForNextTurn),
                    typeof(Active));

                // set persistent id
                EntityManager.SetComponentData(newPlayerEntity, this.MakeUniquePersistentId());

                // cap player name at 30 characters
                string playerName = createPlayerInput.PlayerName;
                if (playerName.Length > 30)
                    playerName = playerName.Substring(0, 30);

                // set name
                EntityManager.SetComponentData(newPlayerEntity, new Name() { Value = playerName });

                // assign controllable entity if possible
                Entity uncontrolledEntity = FindUncontrolledPawn();

                if (uncontrolledEntity == Entity.Null) // if no pawn found, try spawing one
                {
                    uncontrolledEntity = SpawnUncontrolledPawn();
                }

                if (uncontrolledEntity != Entity.Null)
                {
                    EntityManager.SetComponentData(newPlayerEntity, new ControlledEntity() { Value = uncontrolledEntity });
                    EntityManager.SetComponentData(uncontrolledEntity, new Controllable() { CurrentController = newPlayerEntity });
                }

                // set team
                EntityManager.SetComponentData(newPlayerEntity, new Team() { Value = 0 });

                // set 'not ready for next turn'
                EntityManager.SetComponentData(newPlayerEntity, new ReadyForNextTurn() { Value = false });

                // set new player controller as currently active
                EntityManager.SetComponentData(newPlayerEntity, new Active() { Value = true });

                // FOR DEBUGGING ONLY
#if UNITY_EDITOR
                EntityManager.SetName(newPlayerEntity, $"Player({playerName})");
#endif
            }

            if (input is SimInputSetPlayerActive setPlayerActiveInput)
            {
                Entity PlayerEntity = CommonReads.FindPlayerEntity(Accessor, setPlayerActiveInput.PlayerID);

                if (PlayerEntity != Entity.Null)
                {
                    EntityManager.SetComponentData(PlayerEntity, new Active() { Value = setPlayerActiveInput.IsActive });
                }
            }
        }
    }

    private Entity FindUncontrolledPawn()
    {
        Entity uncontrolledEntity = Entity.Null;

        var controllers = GetComponentDataFromEntity<ControlledEntity>(isReadOnly: true);

        Entities
            .ForEach((Entity pawn, in Controllable controllable, in DefaultControllerPrefab defaultController) =>
        {
            if (defaultController.Value != Entity.Null)  // entities with this will have their DefaultController spawned
                return;

            if (!controllers.HasComponent(controllable.CurrentController))
            {
                uncontrolledEntity = pawn;
                return;
            }
        }).Run();

        return uncontrolledEntity;
    }

    private Entity SpawnUncontrolledPawn()
    {
        // get pawn prefab
        if (!TryGetSingleton(out PlayerPawnPrefabReferenceSingletonComponent playerPawnPrefab))
        {
            Log.Error($"No Player Pawn Prefab Singelton, can't spawn player");
            return Entity.Null;
        }

        int spawnPointCount = GetEntityQuery(typeof(SpawnLocationTag)).CalculateEntityCount();

        fix2 spawnPosition;

        if (spawnPointCount == 0) // no spawn point, cannot spawn
        {
            Log.Warning($"No spawn point, creating player pawn at (0,10).");
            spawnPosition = new fix2(0, 10);
        }
        else
        {
            // loop dans les spawn points, eg: 3 spawn points, 5 joueurs: A B C A B
            int playerCount = GetEntityQuery(typeof(PlayerTag)).CalculateEntityCount();
            int spawnPointsIndex = mathX.mod(playerCount, spawnPointCount);
            NativeArray<FixTranslation> spawnPointPositions = GetEntityQuery(typeof(SpawnLocationTag), typeof(FixTranslation)).ToComponentDataArray<FixTranslation>(Allocator.Temp);
            spawnPosition = spawnPointPositions[spawnPointsIndex];
        }

        Entity pawn = EntityManager.Instantiate(playerPawnPrefab.Prefab);
        EntityManager.SetOrAddComponentData(pawn, new FixTranslation() { Value = spawnPosition });

        return pawn;
    }
}

public partial class CommonReads
{
    public static Entity GetPawnController(ISimWorldReadAccessor accessor, Entity pawn)
    {
        return accessor.GetComponentData<Controllable>(pawn).CurrentController;
    }
}
