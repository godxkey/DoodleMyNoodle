using System;
using System.Collections.Generic;

public class SimPawnManager : SimComponentRegistrySingleton<SimPawnManager, SimPawnComponent>,
    ISimInputProcessor,
    ISimEventListener<SimPlayerCreatedEventData>,
    ISimEventListener<SimPlayerDestroyedEventData>
{
    Dictionary<SimPlayerId, SimPawnComponent> _playerPawns = new Dictionary<SimPlayerId, SimPawnComponent>();
    List<ISimPawnInputHandler> _pooledPawnInputHandlersList = new List<ISimPawnInputHandler>(); // this is simply used to reduce allocations

    public ReadOnlyList<SimPawnComponent> Pawns => _components.AsReadOnlyNoAlloc();

    public SimPawnComponent GetPawnOnTile(SimTileId tileId)
    {
        SimPawnComponent pawn = null;
        foreach (SimTransformComponent transform in Simulation.EntitiesWithComponent<SimTransformComponent>())
        {
            if (transform.GetTileId() == tileId && transform.GetComponent(out pawn))
            {
                break;
            }
        }

        return pawn;
    }

    public override void OnSimStart()
    {
        base.OnSimStart();

        SimGlobalEventEmitter.RegisterListener<SimPlayerCreatedEventData>(this);
        SimGlobalEventEmitter.RegisterListener<SimPlayerDestroyedEventData>(this);
    }

    public void ProcessInput(SimInput input)
    {
        if (input is SimPlayerInput playerInput)
        {
            if (_playerPawns.TryGetValue(playerInput.SimPlayerId, out SimPawnComponent pawn))
            {
                if(pawn)
                {
                    pawn.GetComponents<ISimPawnInputHandler>(_pooledPawnInputHandlersList);

                    for (int i = 0; i < _pooledPawnInputHandlersList.Count; i++)
                    {
                        if (_pooledPawnInputHandlersList[i].HandleInput(playerInput))
                        {
                            break;
                        }
                    }

                    _pooledPawnInputHandlersList.Clear();
                }
            }
        }
    }

    public void OnEventRaised(in SimPlayerCreatedEventData eventData)
    {
        _playerPawns.Add(eventData.PlayerInfo.SimPlayerId, null);

        // Assign the first unpossessed pawn to the new player
        //      This will probably be reworked into something more solid later
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].IsPossessed == false)
            {
                AssignPawnToPlayer(_components[i], eventData.PlayerInfo.SimPlayerId);
                break;
            }
        }
    }

    public void OnEventRaised(in SimPlayerDestroyedEventData eventData)
    {
        _playerPawns.Remove(eventData.SimPlayerId);
    }

    internal void AssignPawnToPlayer(SimPawnComponent newPawn, SimPlayerId simPlayerId)
    {
        if (_playerPawns.TryGetValue(simPlayerId, out SimPawnComponent previousPawn))
        {
            if (previousPawn)
                previousPawn.PlayerInControl = SimPlayerId.Invalid;
            if (newPawn)
                newPawn.PlayerInControl = simPlayerId;

            _playerPawns[simPlayerId] = newPawn;
        }
        else
        {
            DebugService.LogError($"[{nameof(SimPawnManager)}] Trying to assign a pawn to an invalid sim player.");
        }
    }
}