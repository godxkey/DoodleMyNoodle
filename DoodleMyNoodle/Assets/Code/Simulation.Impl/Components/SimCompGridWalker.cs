using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimCompGridTransform))]
public class SimCompGridWalker : SimBehaviour, ISimTickable, /*temporary*/ ISimInputHandler
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
                        TryWalkTo(gridTr.tileId + Vector2Int.right);
                    else
                        TryWalkTo(gridTr.tileId + Vector2Int.left);
                    goRight = !goRight;
                    return false;

                case KeyCode.Alpha2:
                    if (goRight)
                        TryWalkTo(gridTr.tileId + Vector2Int.left);
                    else
                        TryWalkTo(gridTr.tileId + Vector2Int.right);
                    goRight = !goRight;
                    return false;

                case KeyCode.RightArrow:
                    TryWalkTo(gridTr.tileId + Vector2Int.right);
                    return false;

                case KeyCode.LeftArrow:
                    TryWalkTo(gridTr.tileId + Vector2Int.left);
                    return false;

                case KeyCode.UpArrow:
                    TryWalkTo(gridTr.tileId + Vector2Int.up);
                    return false;

                case KeyCode.DownArrow:
                    TryWalkTo(gridTr.tileId + Vector2Int.down);
                    return false;
            }
        }

        return false;
    }


    public Fix64 speed;
    public bool hasADestination { get; private set; }
    public List<SimTileId> path;

    public SimTileId tileId => gridTr.tileId;

    public void TryWalkTo(in SimTileId destination)
    {
        hasADestination = SimPathService.instance.GetPathTo(this, destination, ref path);
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
                FixVector3 currentPosition = gridTr.worldPosition;
                FixVector3 targetPosition = targetTile.GetWorldPosition3D();


                // calculate move
                FixVector3 v = targetPosition - currentPosition;
                FixVector3 moveVector = (v.normalized * Simulation.deltaTime * speed).LimitDirection(v);

                // endpoint
                FixVector3 newPosition = gridTr.worldPosition + moveVector;
                SimTileId newTile = SimTileId.FromWorldPosition(newPosition);


                if (newTile != currentTile && SimHelper.CanEntityWalkGoOntoTile(this, newTile) == false)
                {
                    // We're about to change tile but it's occupied, bump!

                    Stop();
                    gridTr.worldPosition = currentTile.GetWorldPosition3D(); // normally, we would have a nice 'bump' animation
                }
                else
                {
                    // Normal move

                    // Process move
                    gridTr.worldPosition = newPosition;

                    UpdatePathAndDestination();
                }
            }
        }
    }

    void UpdatePathAndDestination()
    {
        FixVector3 currentPosition = gridTr.worldPosition;
        FixVector3 targetPathPosition = path[0].GetWorldPosition3D();

        if (tileId == path[0] && FixMath.AlmostEqual(currentPosition, targetPathPosition))
        {
            // we've reached the node
            gridTr.worldPosition = targetPathPosition;
            path.RemoveFirst();

            if (path.Count == 0)
            {
                // we've reached the destination
                Stop();
            }
        }
    }



    SimCompGridTransform gridTr;
    public override void OnAddedToEntityList()
    {
        base.OnAddedToEntityList();
        gridTr = GetComponent<SimCompGridTransform>();
    }
}