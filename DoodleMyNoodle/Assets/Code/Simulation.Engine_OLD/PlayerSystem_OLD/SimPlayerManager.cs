using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimPlayerManager : SimSingleton<SimPlayerManager>, ISimInputProcessor
{
    // NB:  The player entity is not the same thing as the pawn entity
    //      The player entity contains things like:
    //          - a reference to the controlled pawn
    //          - the player's name
    //          - (hypothetical) the time at which the player first joined the simulation
    //      The pawn entity contains things like:
    //          - a world position
    //          - some health, mana, items, etc.

    [SerializeField]
    SimPlayerComponent _playerPrefab;

    public void ProcessInput(SimInput input)
    {
        switch (input)
        {
            case SimInputPlayerCreateOld playerCreate:
            {
                if (playerCreate.SimPlayerInfo == null)
                {
                    DebugService.LogError($"Trying to create a sim player from null data.");
                    return;
                }

                SimEntity playerEntity = Simulation.Instantiate(_playerPrefab.SimEntity);

                if (playerEntity.GetComponent(out SimNameComponent nameComponent))
                {
                    nameComponent.Value = playerCreate.SimPlayerInfo.Name;
                }

                // Raise event
                SimGlobalEventEmitter.RaiseEvent(new SimPlayerCreatedEventData() { PlayerEntity = playerEntity });
                break;
            }

            // fbessette: Pour l'instant, on détruit jamais un joueur de la liste des joueurs 
            //      e.g.: Tony joue dans une game avec ces amis.
            //            Il arrête de joué après quelques temps.
            //            Ces amis continue de jouer sans lui sur la même game.
            //            S'il reviens 1 mois plus tard, il DOIT encore avoir son "personnage" et ses "equipements".
            //case SimInputPlayerDestroy playerDestroy:
            //    break;


            case SimPlayerInput playerInput:
            {
                // valid player ?
                SimPlayerComponent player = SimPlayerHelpersOld.FindPlayerFromId(playerInput.SimPlayerIdOld);
                if (player)
                {
                    HandleInputFromPlayer(player, playerInput);
                }

                break;
            }
        }
    }


    List<ISimPlayerInputHandler> _cachedComponentList = new List<ISimPlayerInputHandler>(); // this is simply used to reduce allocations
    
    public void HandleInputFromPlayer(SimPlayerComponent player, SimPlayerInput input)
    {
        player.GetComponents<ISimPlayerInputHandler>(_cachedComponentList);

        for (int i = 0; i < _cachedComponentList.Count; i++)
        {
            if (_cachedComponentList[i].HandleInput(input))
            {
                break;
            }
        }

        _cachedComponentList.Clear();
    }
}