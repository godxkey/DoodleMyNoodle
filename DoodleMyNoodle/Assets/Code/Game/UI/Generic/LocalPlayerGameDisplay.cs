using Unity.Entities;

public class LocalPlayerGameDisplay : GameMonoBehaviour
{
    protected Entity _localPawn = Entity.Null;
    protected Entity _localController = Entity.Null;

    public override void OnGameUpdate()
    {
        UpdateCurrentPlayerPawn();
    }

      private void UpdateCurrentPlayerPawn()
    {
        if (_localPawn == Entity.Null)
        {
            _localPawn = PlayerHelpers.GetLocalSimPawnEntity(SimWorld);
            _localController = CommonReads.GetPawnController(SimWorld, _localPawn);
        }
    }
}
