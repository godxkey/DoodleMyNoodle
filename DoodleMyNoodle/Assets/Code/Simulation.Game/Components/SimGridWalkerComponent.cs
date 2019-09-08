using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimGridTransformComponent))]
public class SimGridWalkerComponent : SimComponent, ISimTickable, /*temporary*/ ISimPawnInputHandler
{
    static bool goRight = true;

    // TEMPORARY
    public bool HandleInput(SimInput input)
    {
        if (input is SimInputKeycode keyCodeInput && keyCodeInput.state == SimInputKeycode.State.Pressed)
        {
            switch (keyCodeInput.keyCode)
            {
                case KeyCode.Alpha1:
                    if (goRight)
                        TryWalkTo(_gridTr.tileId + Vector2Int.right);
                    else
                        TryWalkTo(_gridTr.tileId + Vector2Int.left);
                    goRight = !goRight;
                    return false;

                case KeyCode.Alpha2:
                    if (goRight)
                        TryWalkTo(_gridTr.tileId + Vector2Int.left);
                    else
                        TryWalkTo(_gridTr.tileId + Vector2Int.right);
                    goRight = !goRight;
                    return false;

                case KeyCode.RightArrow:
                    TryWalkTo(_gridTr.tileId + Vector2Int.right);
                    return false;

                case KeyCode.LeftArrow:
                    TryWalkTo(_gridTr.tileId + Vector2Int.left);
                    return false;

                case KeyCode.UpArrow:
                    TryWalkTo(_gridTr.tileId + Vector2Int.up);
                    return false;

                case KeyCode.DownArrow:
                    TryWalkTo(_gridTr.tileId + Vector2Int.down);
                    return false;
            }
        }

        return false;
    }


    public Fix64 speed;
    public bool hasADestination { get; private set; }
    public List<SimTileId> path;

    public SimTileId tileId => _gridTr.tileId;

    public void TryWalkTo(in SimTileId destination)
    {
        hasADestination = SimPathService.Instance.GetPathTo(this, destination, ref path);
    }

    public void Stop()
    {
        hasADestination = false;
    }

    public void OnSimTick()
    {
        if (hasADestination)
        {
            UpdatePathAndDestination(); // if some external force moved us, we

            if (hasADestination)
            {
                SimTileId currentTile = tileId;
                SimTileId targetTile = path[0];
                FixVector3 currentPosition = _gridTr.worldPosition;
                FixVector3 targetPosition = targetTile.GetWorldPosition3D();


                // calculate move
                FixVector3 v = targetPosition - currentPosition;
                FixVector3 moveVector = (v.normalized * Simulation.DeltaTime * speed).LimitDirection(v);

                // endpoint
                FixVector3 newPosition = _gridTr.worldPosition + moveVector;
                SimTileId newTile = SimTileId.FromWorldPosition(newPosition);


                if (newTile != currentTile && SimHelper.CanEntityWalkGoOntoTile(this, newTile) == false)
                {
                    // We're about to change tile but it's occupied, bump!

                    Stop();
                    _gridTr.worldPosition = currentTile.GetWorldPosition3D(); // normally, we would have a nice 'bump' animation
                }
                else
                {
                    // Normal move

                    // Process move
                    _gridTr.worldPosition = newPosition;

                    UpdatePathAndDestination();
                }
            }
        }
    }

    void UpdatePathAndDestination()
    {
        FixVector3 currentPosition = _gridTr.worldPosition;
        FixVector3 targetPathPosition = path[0].GetWorldPosition3D();

        if (tileId == path[0] && FixMath.AlmostEqual(currentPosition, targetPathPosition))
        {
            // we've reached the node
            _gridTr.worldPosition = targetPathPosition;
            path.RemoveFirst();

            if (path.Count == 0)
            {
                // we've reached the destination
                Stop();
            }
        }
    }


    [System.NonSerialized]
    SimGridTransformComponent _gridTr;
    public override void OnAddedToRuntime()
    {
        _gridTr = GetComponent<SimGridTransformComponent>();
    }
}