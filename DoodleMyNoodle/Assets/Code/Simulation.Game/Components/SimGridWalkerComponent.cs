using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimGridTransformComponent))]
public class SimGridWalkerComponent : SimComponent, ISimTickable
{
    public Fix64 Speed = new Fix64(4);
    public bool HasADestination { get; private set; }
    public List<SimTileId> Path;

    public SimTileId TileId => _gridTr.TileId;

    public void TryWalkTo(in SimTileId destination)
    {
        HasADestination = SimPathService.Instance.GetPathTo(this, destination, ref Path);
    }

    public void Stop()
    {
        HasADestination = false;
    }

    public void OnSimTick()
    {
        if (HasADestination)
        {
            UpdatePathAndDestination(); // if some external force moved us, we

            if (HasADestination)
            {
                SimTileId currentTile = TileId;
                SimTileId targetTile = Path[0];
                FixVector3 currentPosition = _gridTr.WorldPosition;
                FixVector3 targetPosition = targetTile.GetWorldPosition3D();


                // calculate move
                FixVector3 v = targetPosition - currentPosition;
                FixVector3 moveVector = (v.normalized * Simulation.DeltaTime * Speed).LimitDirection(v);

                // endpoint
                FixVector3 newPosition = _gridTr.WorldPosition + moveVector;
                SimTileId newTile = SimTileId.FromWorldPosition(newPosition);


                if (newTile != currentTile && SimHelper.CanEntityWalkGoOntoTile(this, newTile) == false)
                {
                    // We're about to change tile but it's occupied, bump!

                    Stop();
                    _gridTr.WorldPosition = currentTile.GetWorldPosition3D(); // normally, we would have a nice 'bump' animation
                }
                else
                {
                    // Normal move

                    // Process move
                    _gridTr.WorldPosition = newPosition;

                    UpdatePathAndDestination();
                }
            }
        }
    }

    void UpdatePathAndDestination()
    {
        FixVector3 currentPosition = _gridTr.WorldPosition;
        FixVector3 targetPathPosition = Path[0].GetWorldPosition3D();

        if (TileId == Path[0] && FixMath.AlmostEqual(currentPosition, targetPathPosition))
        {
            // we've reached the node
            _gridTr.WorldPosition = targetPathPosition;
            Path.RemoveFirst();

            if (Path.Count == 0)
            {
                // we've reached the destination
                Stop();
            }
        }
    }



    #region Component Caching
    [System.NonSerialized] SimGridTransformComponent _gridTr;
    public override void OnAddedToRuntime()
    {
        base.OnAddedToRuntime();
        _gridTr = GetComponent<SimGridTransformComponent>();
    }
    #endregion
}