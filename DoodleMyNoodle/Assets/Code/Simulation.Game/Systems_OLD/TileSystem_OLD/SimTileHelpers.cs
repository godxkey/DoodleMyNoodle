
public static class SimTileHelpers
{
    public static bool CanEntityWalkGoOntoTile(SimEntity entity, in SimTileId_OLD tile)
    {
        SimGridWalkerComponent walkerComponent = entity.GetComponent<SimGridWalkerComponent>();
        if (walkerComponent == null)
            return false;
        return CanEntityWalkOntoTile(walkerComponent, tile);
    }

    public static bool CanEntityWalkOntoTile(SimGridWalkerComponent walker, in SimTileId_OLD tile)
    {
        return SimTileManager.Instance.IsTileWalkable(tile)
            && GetPawnOnTile(tile) == null
            && GetObstacleOnTile(tile) == null;
    }

    /// <summary>
    /// Returns the first pawn found on the matching tile (or null if none found)
    /// </summary>
    public static SimEntity GetPawnOnTile(SimTileId_OLD tileId)
    {
        foreach (SimPawnComponent pawn in Simulation.EntitiesWithComponent<SimPawnComponent>())
        {
            if(pawn.TryGetComponent(out SimTransformComponent transform))
            {
                if (transform.GetTileId() == tileId)
                {
                    return pawn.SimEntity;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Returns the first pawn found on the matching tile (or null if none found)
    /// </summary>
    public static SimEntity GetObstacleOnTile(SimTileId_OLD tileId)
    {
        foreach (SimObstacleComponent obstacle in Simulation.EntitiesWithComponent<SimObstacleComponent>())
        {
            if (obstacle.TryGetComponent(out SimTransformComponent transform))
            {
                if (transform.GetTileId() == tileId)
                {
                    return obstacle.SimEntity;
                }
            }
        }

        return null;
    }
}