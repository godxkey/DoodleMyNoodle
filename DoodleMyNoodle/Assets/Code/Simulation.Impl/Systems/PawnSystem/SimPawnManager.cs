
public class SimPawnManager : SimCompSingleton<SimPawnManager>
{
    // this system is pretty bare bones and will need to be done correctly

    public SimEntity GetPawnOnTile(SimTileId tileId)
    {
        SimEntity pawn = null;

        Simulation.ForEveryEntityWithComponent((SimGridTransformComponent gridTr) =>
        {
            if(gridTr.tileId == tileId)
            {
                pawn = gridTr.simEntity;
                return false; // equivalent to break;
            }
            return true;
        });

        return pawn;
    }
}
