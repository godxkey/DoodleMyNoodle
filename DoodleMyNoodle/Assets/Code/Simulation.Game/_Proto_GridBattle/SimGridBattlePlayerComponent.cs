using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimTeamMemberComponent))]
[RequireComponent(typeof(SimPawnControllerComponent))]
public class SimGridBattlePlayerComponent : SimEventComponent, 
    ISimPlayerInputHandler,
    ISimTargetPawnChangeListener
{
    public bool HandleInput(SimPlayerInput input)
    {
        if (SimTurnManager.Instance.IsMyTurn(_team.Team) && input is SimInputKeycode keyCodeInput && keyCodeInput.state == SimInputKeycode.State.Pressed)
        {
            switch (keyCodeInput.keyCode)
            {
                case KeyCode.RightArrow:
                    _pawnGridWalker.TryWalkTo(_pawnGridWalker.TileId + Vector2Int.right);
                    return false;

                case KeyCode.LeftArrow:
                    _pawnGridWalker.TryWalkTo(_pawnGridWalker.TileId + Vector2Int.left);
                    return false;

                case KeyCode.UpArrow:
                    _pawnGridWalker.TryWalkTo(_pawnGridWalker.TileId + Vector2Int.up);
                    return false;

                case KeyCode.DownArrow:
                    _pawnGridWalker.TryWalkTo(_pawnGridWalker.TileId + Vector2Int.down);
                    return false;
            }
        }

        return false;
    }

    void ISimTargetPawnChangeListener.OnTargetPawnChanged()
    {
        UpdateCachedPawnComponents();
    }

    #region Pawn Component Caching
    [System.NonSerialized] SimGridWalkerComponent _pawnGridWalker;
    void UpdateCachedPawnComponents()
    {
        if (_targetPawn.TargetPawn)
        {
            _pawnGridWalker = _targetPawn.TargetPawn.GetComponent<SimGridWalkerComponent>();
        }
    }
    #endregion

    #region Component Caching
    [System.NonSerialized] SimTeamMemberComponent _team;
    [System.NonSerialized] SimPawnControllerComponent _targetPawn;
    public override void OnAddedToRuntime()
    {
        base.OnAddedToRuntime();
        _team = GetComponent<SimTeamMemberComponent>();
        _targetPawn = GetComponent<SimPawnControllerComponent>();

        UpdateCachedPawnComponents();
    }
    #endregion
}
