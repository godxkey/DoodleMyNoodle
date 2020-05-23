using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GamePresentationCache : GameSystem<GamePresentationCache>
{
    public Entity LocalPawn = Entity.Null;
    public Entity LocalController = Entity.Null;

    public override bool SystemReady => true;

    public override void OnGameUpdate()
    {
        UpdateCurrentPlayerPawn();
    }

    private void UpdateCurrentPlayerPawn()
    {
        LocalPawn = PlayerHelpers.GetLocalSimPawnEntity(SimWorld);
        LocalController = CommonReads.GetPawnController(SimWorld, LocalPawn);
    }
}
