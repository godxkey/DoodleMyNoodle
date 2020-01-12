using System;
using System.Collections.Generic;

public class SimPawnManager : SimEventSingleton<SimPawnManager>,
    ISimInputProcessor,
    ISimEventListener<SimPlayerCreatedEventData>,
    ISimEventListener<SimPlayerDestroyedEventData>
{
    [System.Serializable]
    struct SerializedData
    {
        public Dictionary<SimPlayerId, SimPawnComponent> PlayerPawnsMap;
    }

    public override void OnSimStart()
    {
        base.OnSimStart();

        SimGlobalEventEmitter.RegisterListener<SimPlayerCreatedEventData>(this);
        SimGlobalEventEmitter.RegisterListener<SimPlayerDestroyedEventData>(this);
    }

    List<ISimPawnInputHandler> _cachedComponentList = new List<ISimPawnInputHandler>(); // this is simply used to reduce allocations
    public void ProcessInput(SimInput input)
    {
        if (input is SimPlayerInput playerInput)
        {
            if (_data.PlayerPawnsMap.TryGetValue(playerInput.SimPlayerId, out SimPawnComponent pawn))
            {
                if(pawn)
                {
                    pawn.GetComponents<ISimPawnInputHandler>(_cachedComponentList);

                    for (int i = 0; i < _cachedComponentList.Count; i++)
                    {
                        if (_cachedComponentList[i].HandleInput(playerInput))
                        {
                            break;
                        }
                    }

                    _cachedComponentList.Clear();
                }
            }
        }
    }

    public void OnEventRaised(in SimPlayerCreatedEventData eventData)
    {
        _data.PlayerPawnsMap.Add(eventData.PlayerInfo.SimPlayerId, null);

        // Assign the first unpossessed pawn to the new player
        //      This will probably be reworked into something more solid later
        foreach (SimPawnComponent pawn in Simulation.EntitiesWithComponent<SimPawnComponent>())
        {
            if (pawn.IsPossessed == false)
            {
                AssignPawnToPlayer(pawn, eventData.PlayerInfo.SimPlayerId);
                break;
            }
        }
    }

    public void OnEventRaised(in SimPlayerDestroyedEventData eventData)
    {
        _data.PlayerPawnsMap.Remove(eventData.SimPlayerId);
    }

    internal void AssignPawnToPlayer(SimPawnComponent newPawn, SimPlayerId simPlayerId)
    {
        if (_data.PlayerPawnsMap.TryGetValue(simPlayerId, out SimPawnComponent previousPawn))
        {
            if (previousPawn)
                previousPawn.PlayerInControl = SimPlayerId.Invalid;
            if (newPawn)
                newPawn.PlayerInControl = simPlayerId;

            _data.PlayerPawnsMap[simPlayerId] = newPawn;
        }
        else
        {
            DebugService.LogError($"[{nameof(SimPawnManager)}] Trying to assign a pawn to an invalid sim player.");
        }
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
        PlayerPawnsMap = new Dictionary<SimPlayerId, SimPawnComponent>()
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