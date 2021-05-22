using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public static class PlayerHelpers
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

    public static Entity GetLocalSimPawnEntity(ExternalSimWorldAccessor simulationWorld)
    {
        Entity localPlayerEntity = PlayerHelpers.GetLocalSimPlayerEntity(simulationWorld);

        // player is controlling an entity
        if (localPlayerEntity != Entity.Null &&
            simulationWorld.TryGetComponent(localPlayerEntity, out ControlledEntity controlledEntity))
        {
            // entity still exists and is controllable
            if (simulationWorld.Exists(controlledEntity.Value) &&
                simulationWorld.HasComponent<Controllable>(controlledEntity.Value))
            {
                return controlledEntity.Value;
            }
        }

        return Entity.Null;
    }

    public static Entity GetLocalSimPlayerEntity(ExternalSimWorldAccessor simulationWorld)
    {
        if (PlayerRepertoireSystem.Instance == null)
            return Entity.Null;

        return PlayerHelpers.GetSimPlayerFromPlayer(PlayerRepertoireSystem.Instance.GetLocalPlayerInfo(), simulationWorld);
    }

    public static PlayerInfo GetPlayerFromSimPlayer(Entity playerEntity, ExternalSimWorldAccessor simWorldAccessor)
    {
        if (simWorldAccessor.HasComponent<PersistentId>(playerEntity))
            return GetPlayerFromSimPlayer(simWorldAccessor.GetComponent<PersistentId>(playerEntity));
        else
            return null;
    }

    public static PlayerInfo GetPlayerFromSimPlayer(PersistentId simPlayerId)
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

    public static Entity GetSimPlayerFromPlayer(PlayerInfo playerInfo, ExternalSimWorldAccessor simulationWorld)
    {
        Entity result = Entity.Null;
        simulationWorld.Entities
            .WithAllReadOnly<PlayerTag, PersistentId>()
            .ForEach((Entity playerEntity, ref PersistentId simPlayerId) =>
            {
                if (simPlayerId == playerInfo.SimPlayerId)
                {
                    result = playerEntity;
                    return;
                }
            });

        return result;
    }
}