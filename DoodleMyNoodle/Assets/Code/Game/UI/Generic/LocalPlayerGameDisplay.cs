using Unity.Entities;

public class LocalPlayerGameDisplay : GameMonoBehaviour
{
    protected static Entity s_localPawn = Entity.Null;
    protected static Entity s_localController = Entity.Null;

    public override void OnGameUpdate()
    {
        UpdateCurrentPlayerPawn();
    }

      private void UpdateCurrentPlayerPawn()
    {
        Entity currentLocalPawn = PlayerHelpers.GetLocalSimPawnEntity(SimWorld);
        if(currentLocalPawn != s_localPawn)
        {
            s_localPawn = currentLocalPawn;
        }

        Entity currentLocalController = CommonReads.GetPawnController(SimWorld, s_localPawn);
        if (currentLocalController != s_localController)
        {
            s_localController = currentLocalController;
        }
    }
}
