using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimPlayerManager : SimSingleton<SimPlayerManager>, ISimInputProcessor
{
    [SerializeField] SimPlayerId _nextPlayerId;
    [SerializeField] List<SimPlayerInfo> _players = new List<SimPlayerInfo>();

    /// <summary>
    /// Iterate over this with "foreach(ISimPlayerInfo playerInfo in GetPlayers())"
    /// </summary>
    public ReadOnlyList<SimPlayerInfo, ISimPlayerInfo> Players => new ReadOnlyList<SimPlayerInfo, ISimPlayerInfo>(_players);

    public override void OnSimAwake()
    {
        base.OnSimAwake();

        _nextPlayerId = SimPlayerId.FirstValid;
    }

    public ISimPlayerInfo GetSimPlayerInfo(in SimPlayerId playerId)
    {
        return GetSimPlayerInfoInternal(playerId);
    }

    SimPlayerInfo GetSimPlayerInfoInternal(in SimPlayerId playerId)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].SimPlayerId == playerId)
            {
                return _players[i];
            }
        }

        return null;
    }

    public void ProcessInput(SimInput input)
    {
        switch (input)
        {
            case SimInputPlayerCreate playerCreate:
            {
                if (playerCreate.SimPlayerInfo == null)
                {
                    DebugService.LogError($"Trying to create a sim player from null data.");
                    return;
                }

                SimPlayerInfo newPlayerInfo = new SimPlayerInfo(playerCreate.SimPlayerInfo);
                newPlayerInfo.SimPlayerId = _nextPlayerId;

                _players.Add(newPlayerInfo);

                _nextPlayerId++;

                // Raise event
                SimGlobalEventEmitter.RaiseEvent(new SimPlayerCreatedEventData() { PlayerInfo = newPlayerInfo });
                break;
            }


            case SimInputPlayerUpdate playerUpdate:
            {
                SimPlayerInfo playerInfo = GetSimPlayerInfoInternal(playerUpdate.PlayerId);

                if (playerInfo == null)
                {
                    DebugService.LogError($"Trying to update an unregistered player's info in the SimPlayerManager: {playerUpdate.PlayerInfo.Name}");
                    return;
                }

                playerInfo.Name = playerUpdate.PlayerInfo.Name;

                // Raise event
                SimGlobalEventEmitter.RaiseEvent(new SimPlayerUpdatedEvent() { PlayerInfo = playerInfo });
                break;
            }

            // fbessette: Pour l'instant, on détruit jamais un joueur de la liste des joueurs 
            //      e.g.: Tony joue dans une game avec ces amis.
            //            Il arrête de joué après quelques temps.
            //            Ces amis continue de jouer sans lui sur la même game.
            //            S'il reviens 1 mois plus tard, il DOIT encore avoir son "personnage" et ses "equipements".
            //case SimInputPlayerDestroy playerDestroy:
            //    break;
        }
    }
}