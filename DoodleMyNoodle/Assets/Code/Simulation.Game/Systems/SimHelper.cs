public static class SimHelper
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
            && SimPawnManager.Instance.GetPawnOnTile(tile) == null;
    }
}