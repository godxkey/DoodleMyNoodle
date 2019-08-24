public static class SimHelper
{
    public static bool CanEntityWalkGoOntoTile(SimEntity entity, in SimTileId tile)
    {
        SimGridWalkerComponent walkerComponent = entity.GetComponent<SimGridWalkerComponent>();
        if (walkerComponent == null)
            return false;
        return CanEntityWalkGoOntoTile(walkerComponent, tile);
    }

    public static bool CanEntityWalkGoOntoTile(SimGridWalkerComponent walker, in SimTileId tile)
    {
        return SimTileManager.instance.IsTileWalkable(tile)
            && SimPawnManager.instance.GetPawnOnTile(tile) == null;
    }
}