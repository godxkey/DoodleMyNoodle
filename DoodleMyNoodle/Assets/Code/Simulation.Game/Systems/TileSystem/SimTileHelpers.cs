
public static class SimTileHelpers
{
    public static bool CanEntityWalkGoOntoTile(SimEntity entity, in SimTileId tile)
    {
        SimGridWalkerComponent walkerComponent = entity.GetComponent<SimGridWalkerComponent>();
        if (walkerComponent == null)
            return false;
        return CanEntityWalkOntoTile(walkerComponent, tile);
    }

    public static bool CanEntityWalkOntoTile(SimGridWalkerComponent walker, in SimTileId tile)
    {
        return SimTileManager.Instance.IsTileWalkable(tile)
            && GetPawnOnTile(tile) == null;
    }

    /// <summary>
    /// Returns the first pawn found on the matching tile (or null if none found)
    /// </summary>
    public static SimEntity GetPawnOnTile(SimTileId tileId)
    {
        foreach (SimPawnComponent pawn in Simulation.EntitiesWithComponent<SimPawnComponent>())
        {
            if(pawn.GetComponent(out SimTransformComponent transform))
            {
                if (transform.GetTileId() == tileId)
                {
                    return pawn.SimEntity;
                }
            }
        }

        return null;
    }
}