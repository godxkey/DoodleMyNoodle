using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimPlayerManager : SimSingleton<SimPlayerManager>, ISimInputProcessor
{
    [System.Serializable]
    struct SerializedData
    {
        public SimPlayerId NextPlayerId;
        public List<SimPlayerInfo> Players;
    }

    /// <summary>
    /// You can iterate over this with "foreach(ISimPlayerInfo playerInfo in GetPlayers())"
    /// </summary>
    public ReadOnlyList<SimPlayerInfo, ISimPlayerInfo> Players => new ReadOnlyList<SimPlayerInfo, ISimPlayerInfo>(_data.Players);

    public override void OnSimAwake()
    {
        base.OnSimAwake();

        _data.NextPlayerId = SimPlayerId.FirstValid;
    }

    public ISimPlayerInfo GetSimPlayerInfo(in SimPlayerId playerId)
    {
        return GetSimPlayerInfoInternal(playerId);
    }

    SimPlayerInfo GetSimPlayerInfoInternal(in SimPlayerId playerId)
    {
        for (int i = 0; i < _data.Players.Count; i++)
        {
            if (_data.Players[i].SimPlayerId == playerId)
            {
                return _data.Players[i];
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
                newPlayerInfo.SimPlayerId = _data.NextPlayerId;

                _data.Players.Add(newPlayerInfo);

                _data.NextPlayerId++;

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

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
        Players = new List<SimPlayerInfo>()
    };

    public override void SerializeToDataStack(SimComponentDataStack dataStack)
    {
        base.SerializeToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void DeserializeFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.DeserializeFromDataStack(dataStack);
    }
    #endregion
}