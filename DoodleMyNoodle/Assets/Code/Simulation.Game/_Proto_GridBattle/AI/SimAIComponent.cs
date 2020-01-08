using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimPawnControllerComponent))]
[RequireComponent(typeof(SimTeamMemberComponent))]
public class SimAIComponent : SimComponent, 
    ISimTickable,
    ISimTargetPawnChangeListener
{
    bool _turnPlayed = false;

    void ISimTickable.OnSimTick()
    {
        if (SimTurnManager.Instance.IsMyTurn(_team.Team))
        {
            if (!_turnPlayed)
            {
                _turnPlayed = true;

                if (_pawnGridWalker)
                {
                    Fix64 randomDecision = Simulation.Random.Range(0, 3);

                    if (randomDecision < 1)
                    {
                        _pawnGridWalker.TryWalkTo(_pawnGridWalker.TileId + Vector2Int.right);
                    }
                    else if (randomDecision < 2)
                    {
                        _pawnGridWalker.TryWalkTo(_pawnGridWalker.TileId + Vector2Int.left);
                    }
                    else
                    {
                        _pawnGridWalker.TryWalkTo(_pawnGridWalker.TileId + Vector2Int.down);
                    }
                }
            }
        }
        else
        {
            _turnPlayed = false;
        }
    }

    void ISimTargetPawnChangeListener.OnTargetPawnChanged()
    {
        UpdateCachedPawnComponents();
    }

    #region Pawn Component Caching
    [System.NonSerialized] SimGridWalkerComponent _pawnGridWalker;
    void UpdateCachedPawnComponents()
    {
        if(_targetPawn.TargetPawn)
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
