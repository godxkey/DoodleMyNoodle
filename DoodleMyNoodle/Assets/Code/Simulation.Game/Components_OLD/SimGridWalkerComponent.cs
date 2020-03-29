using System;
using System.Collections.Generic;
using UnityEngine;

public struct WalkedOnTileEventData
{

}


[RequireComponent(typeof(SimTransformComponent))]
public class SimGridWalkerComponent : SimEventComponent, ISimTickable
{
    [System.Serializable]
    struct SerializedData
    {
        public fix Speed;

        public bool HasADestination;

        public List<SimTileId_OLD> Path;
        
        public SimEvent<WalkedOnTileEventData> OnWalkedOnTileEvent;
    }

    public fix Speed { get => _data.Speed; internal set => _data.Speed = value; }
    public bool HasADestination => _data.HasADestination;
    public SimTileId_OLD TileId => SimTransform.GetTileId();
    public SimEvent<WalkedOnTileEventData> OnWalkedOnTileEvent => _data.OnWalkedOnTileEvent;

    public bool WantsToWalk = false;
    public bool ChoiceMade = false;

    private Action<SimTileId_OLD> _currentDestinationFoundCallback = null;

    public override void OnSimAwake()
    {
        base.OnSimAwake();

        _data.OnWalkedOnTileEvent = CreateLocalEvent<WalkedOnTileEventData>();
    }

    public void TryWalkTo(in SimTileId_OLD destination)
    {
        if (destination == TileId)
            return;

        _data.HasADestination = SimPathService.Instance.GetPathTo(this, destination, ref _data.Path);
    }

    internal void Stop()
    {
        _data.HasADestination = false;
    }

    public void OnSimTick()
    {
        if (HasADestination)
        {
            UpdatePathAndDestination(); // if some external force moved us, we

            if (HasADestination)
            {
                SimTileId_OLD currentTile = TileId;
                SimTileId_OLD targetTile = _data.Path[0];
                fix3 currentPosition = SimTransform.WorldPosition;
                fix3 targetPosition = targetTile.GetWorldPosition3D();


                // calculate move
                fix3 v = targetPosition - currentPosition;
                fix3 moveVector = (v.normalized * Simulation.DeltaTime * _data.Speed).LimitDirection(v);

                // endpoint
                fix3 newPosition = SimTransform.WorldPosition + moveVector;
                SimTileId_OLD newTile = SimTileId_OLD.FromWorldPosition(newPosition);


                if (newTile != currentTile && SimTileHelpers.CanEntityWalkOntoTile(this, newTile) == false)
                {
                    // We're about to change tile but it's occupied, bump!

                    Stop();
                    SimTransform.WorldPosition = currentTile.GetWorldPosition3D(); // normally, we would have a nice 'bump' animation
                }
                else
                {
                    // Normal move

                    // Process move
                    SimTransform.WorldPosition = newPosition;

                    UpdatePathAndDestination();
                }
            }
        }
    }

    public void OnRequestToWalk(Action<SimTileId_OLD> OnDestinationFound)
    {
        _currentDestinationFoundCallback = OnDestinationFound;
        ChoiceMade = false;
        WantsToWalk = true;
    }

    public void OnCancelWalkRequest()
    {
        _currentDestinationFoundCallback = null;
        WantsToWalk = false;
        ChoiceMade = true;
    }

    public void OnDestinationChoosen(SimTileId_OLD simTileId)
    {
        if (WantsToWalk)
        {
            _currentDestinationFoundCallback?.Invoke(simTileId);
            OnCancelWalkRequest();
        }
    }

    void UpdatePathAndDestination()
    {
        fix3 currentPosition = SimTransform.WorldPosition;
        fix3 targetPathPosition = _data.Path[0].GetWorldPosition3D();

        if (TileId == _data.Path[0] && fixMath.AlmostEqual(currentPosition, targetPathPosition))
        {
            // we've reached the node
            SimTransform.WorldPosition = targetPathPosition;
            _data.Path.RemoveFirst();

            if (_data.Path.Count == 0)
            {
                // we've reached the destination
                Stop();
            }

            _data.OnWalkedOnTileEvent.Raise(new WalkedOnTileEventData());
        }
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [CCC.InspectorDisplay.AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}