public static class SimHelper
{
    public static bool CanEntityWalkGoOntoTile(SimEntity entity, in SimTileId tile)
    {
        SimCompGridWalker walkerComponent = entity.GetComponent<SimCompGridWalker>();
        if (walkerComponent == null)
            return false;
        return CanEntityWalkGoOntoTile(walkerComponent, tile);
    }

    public static bool CanEntityWalkGoOntoTile(SimCompGridWalker walker, in SimTileId tile)
    {
        return SimTileManager.instance.IsTileWalkable(tile)
            && SimPawnManager.instance.GetPawnOnTile(tile) == null;
    }
}