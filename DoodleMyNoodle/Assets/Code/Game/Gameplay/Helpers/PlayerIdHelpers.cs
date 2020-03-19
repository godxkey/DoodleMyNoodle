using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public static class PlayerIdHelpers
{
    public static PlayerId GetLocalPlayerID()
    {
        return PlayerRepertoireSystem.Instance.GetLocalPlayerInfo().PlayerId;
    }

    public static PlayerInfo GetLocalPlayerInfo()
    {
        if (PlayerRepertoireSystem.Instance == null)
            return null;

        return PlayerRepertoireSystem.Instance.GetLocalPlayerInfo();
    }

    public static PlayerInfo GetPlayerInfo(INetworkInterfaceConnection connection)
    {
        if (connection == null)
            return GetLocalPlayerInfo();

        if (PlayerRepertoireServer.Instance != null)
        {
            return PlayerRepertoireServer.Instance.GetPlayerInfo(connection);
        }
        else
        {
            return PlayerRepertoireClient.Instance.GetServerPlayerInfo();
        }
    }

    public static Entity GetLocalSimPawnEntity(World simulationWorld)
    {
        Entity localPlayerEntity = PlayerIdHelpers.GetLocalSimPlayerEntity(simulationWorld);

        if (localPlayerEntity != Entity.Null)
        {
            if(simulationWorld.EntityManager.TryGetComponentData(localPlayerEntity, out ControlledEntity controlledEntity))
            {
                return controlledEntity.Value;
            }
        }

        return Entity.Null;
    }

    public static Entity GetLocalSimPlayerEntity(World simulationWorld)
    {
        if (PlayerRepertoireSystem.Instance == null)
            return Entity.Null;

        return PlayerIdHelpers.GetSimPlayerFromPlayer(PlayerRepertoireSystem.Instance.GetLocalPlayerInfo(), simulationWorld);
    }

    public static PlayerInfo GetPlayerFromSimPlayer(Entity playerEntity, SimWorldAccessor simWorldAccessor)
    {
        if (simWorldAccessor.HasComponent<PersistentId>(playerEntity))
            return GetPlayerFromSimPlayer(simWorldAccessor.GetComponentData<PersistentId>(playerEntity));
        else
            return null;
    }

    public static PlayerInfo GetPlayerFromSimPlayer(in PersistentId simPlayerId)
    {
        if (PlayerRepertoireSystem.Instance == null)
            return null;

        foreach (PlayerInfo player in PlayerRepertoireSystem.Instance.Players)
        {
            if (player.SimPlayerId == simPlayerId)
            {
                return player;
            }
        }

        return null;
    }

    public static Entity GetSimPlayerFromPlayer(PlayerInfo playerInfo, World simulationWorld)
    {
        // TODO fbessette: cache this query

        EntityManager simEntityManager = simulationWorld.EntityManager;
        using (EntityQuery query = simEntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerTag>(),
            ComponentType.ReadOnly<PersistentId>()))
        {
            using (NativeArray<Entity> simPlayers = query.ToEntityArray(Allocator.TempJob))
            {
                foreach (Entity playerEntity in simPlayers)
                {
                    if (simEntityManager.GetComponentData<PersistentId>(playerEntity) == playerInfo.SimPlayerId)
                    {
                        return playerEntity;
                    }
                }
            }
        }

        return Entity.Null;
    }
}