using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimAIComponent : SimComponent, ISimTickable
{
    bool _turnPlayed = false;

    void ISimTickable.OnSimTick()
    {
        if (SimTurnManager.Instance.IsMyTurn(_team.Team))
        {
            if (!_turnPlayed)
            {
                _turnPlayed = true;

                Fix64 randomDecision = Simulation.Random.Range(0,3);

                if (randomDecision < 1)
                {
                    _gridWalker.TryWalkTo(_gridWalker.TileId + Vector2Int.right);
                }
                else if (randomDecision < 2)
                {
                    _gridWalker.TryWalkTo(_gridWalker.TileId + Vector2Int.left);
                }
                else
                {
                    _gridWalker.TryWalkTo(_gridWalker.TileId + Vector2Int.down);
                }
            }
        }
        else
        {
            _turnPlayed = false;
        }
    }

    #region Component Caching
    [System.NonSerialized] SimGridWalkerComponent _gridWalker;
    [System.NonSerialized] SimTeamMemberComponent _team;
    public override void OnAddedToRuntime()
    {
        base.OnAddedToRuntime();
        _gridWalker = GetComponent<SimGridWalkerComponent>();
        _team = GetComponent<SimTeamMemberComponent>();
    }
    #endregion
}
